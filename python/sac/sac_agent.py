from copy import deepcopy
from itertools import chain
from dataclasses import dataclass
import os
import time

import numpy as np
import torch
from torch.optim import Adam
from torch import nn

from .sac_core import TrainingParameters, create_mlp, QNet, Policy, ReplayBuffer


def get_obs_dim(env):
    return env.observation_space.shape[0]

def get_act_dim(env):
    return env.action_space.shape[0]

def get_act_high(env):
    return env.action_space.high[0] ## FIXME : doesnt support different highs in action dimensions

def get_random_act(env):
    return env.action_space.sample()

class SACAgent:
    def __init__(self, env, policy_hidden_sizes=(256, 256), qnets_hidden_sizes=(256, 256)) -> None:
        self.env = env
        self.policy = Policy(env.get_obs_dim(), env.get_act_dim(), env.get_act_high())
        self.q1 = QNet(env.get_obs_dim(), env.get_act_dim())
        self.q2 = QNet(env.get_obs_dim(), env.get_act_dim())
        self.q1_targ = deepcopy(self.q1)
        self.q2_targ = deepcopy(self.q2)
        self.pi_optimizer = Adam(self.policy.parameters())
        self.q1_optimizer = Adam(self.q1.parameters())
        self.q2_optimizer = Adam(self.q2.parameters())
        self.param = TrainingParameters()
        self.replay_buffer = None

        # prevent optimizers to modify target qnets (should only be changed w/ polyak)
        for p in chain(self.q1_targ.parameters(), self.q2_targ.parameters()):
            p.requires_grad = False

    ## TODO re-add agent testing, logging, start_step, update_after
    def train(self, seed=None, from_epoch=0, **training_params):
        self.param.update(training_params)
        target_frame_duration = self.env.get_target_frame_duration()
        start_time = 0  # initial value doesn't matter as long as it's < time()

        if self.replay_buffer is None:
            # make sure here replay buffer size is multiple of agent size to simplify datastoring
            self.replay_buffer = ReplayBuffer(self.env.get_obs_dim(), self.env.get_act_dim(),
                                              self.param.replay_size - self.param.replay_size%self.env.get_agent_number())
        ep_len = np.zeros((self.env.get_agent_number(),))
        obs, *_ = self.env.reset()

        for t in range(from_epoch*self.param.epoch_len, self.param.steps, self.env.get_agent_number()):
            ep_len += 1
            # choose action at random or from policy
            if t > self.param.start_steps:
                act = self.get_action(obs)
            else:
                act = self.env.get_random_act()

            # respect env target framerate
            elapsed_time = time.time() - start_time
            sleep_duration = target_frame_duration - elapsed_time
            if sleep_duration > 0:
                time.sleep(sleep_duration)

            # act according to choosen action
            obs2, rew, terminated, truncated, *_ = self.env.step(act)
            start_time = time.time()
            done = terminated | truncated | (ep_len > self.param.max_episode_len)
            start_time = time.time()

            self.replay_buffer.record(obs, act, obs2, rew, done)
            obs = obs2

            # reset environment and ep if episode is finished
            if np.any(done):
                obs, *_ = self.env.reset(done)
                ep_len[done] = 0

            # show epoch stats and save checkpoint on epoch end
            if t>0 and t%self.param.epoch_len < self.env.get_agent_number():
                avg_rew, avg_len = self.test()
                print(f"=== epoch {t//self.param.epoch_len} || avg rew : {avg_rew} || avg len : {avg_len}")
                self.checkpoint(t)

            # update policy and qnet when needed
            if t % self.param.update_every < self.env.get_agent_number() and t >= self.param.update_after:
                self.env.pause()
                for _ in range(self.param.update_every):
                    batch = self.replay_buffer.sample(self.param.batch_size)
                    self.update(batch)
                self.env.resume()
                start_time = time.time() ## need to wait a bit after resuming to be sure the env is ready

            


    def update(self, batch):
        self.q1_optimizer.zero_grad()
        self.q2_optimizer.zero_grad()
        q1_loss, q2_loss = self.compute_loss_q(batch)
        q1_loss.backward()
        q2_loss.backward()
        self.q1_optimizer.step()
        self.q2_optimizer.step()

        for p in chain(self.q1.parameters(), self.q2.parameters()):  # FIXME : equivalent to with no_grad() around q_pi in loss_pi ?
            p.require_grad = False

        self.pi_optimizer.zero_grad()
        pi_loss = self.compute_loss_pi(batch)
        pi_loss.backward()
        self.pi_optimizer.step()

        for p in chain(self.q1.parameters(), self.q2.parameters()):
            p.require_grad = True

        with torch.no_grad():
            for p, p_targ in zip (chain(self.q1.parameters(), self.q2.parameters()),
                                    chain(self.q1_targ.parameters(), self.q2_targ.parameters())):
                p_targ.mul_(self.param.polyak)
                p_targ.add_(p.mul(1-self.param.polyak))
                
    def get_action(self, obs, probabilistic=True): ## TODO rename this
        with torch.no_grad():
            a, _ = self.policy(torch.as_tensor(obs), with_log_prob=False, probabilistic=probabilistic)
            return a.numpy()
        
    def compute_loss_pi(self, batch):
        obs = batch["obs"]
        act, logp = self.policy(obs)
        q_pi = torch.min(
            self.q1(obs, act),
            self.q2(obs, act)
        )
        return -(q_pi-self.param.alpha*logp).mean()
        # minus denotes the fact that we perform a gradient *ascent*

    def compute_loss_q(self, batch):
        obs, act, obs2, rew, done = batch["obs"], batch["act"], batch["obs2"], batch["rew"], batch["done"]
        
        with torch.no_grad():  # no grad while computing target
            pi_act, pi_logp = self.policy(obs2)
            min_q_pi = torch.min(
                self.q1_targ(obs2, pi_act),
                self.q2_targ(obs2, pi_act)
            )
            target = rew + self.param.gamma * (1-done) * (min_q_pi-self.param.alpha*pi_logp)

        return ((self.q1(obs, act) - target)**2).mean(), ((self.q2(obs, act) - target)**2).mean()
    
    def test(self, max_ep=10):
        obs, *_ = self.env.reset()
        ep = 0
        ep_len = 0
        tot_rew = 0
        tot_len = 0
        
        while ep < max_ep:
            act = self.get_action(obs)
            obs, rew, terminated, truncated, *_ = self.env.step(act)
            done = terminated or truncated
            ep_len += 1
            tot_len += 1
            tot_rew += rew

            if done or ep_len >= self.param.max_episode_len:
                obs, *_ = self.env.reset()
                ep_len = 0
                ep += 1
        return tot_rew / max_ep, tot_len / max_ep


    def checkpoint(self, t):
        if not os.path.exists(f"save/{self.env.get_name()}/"):
            os.makedirs(f"save/{self.env.get_name()}/")
        torch.save({
            'epoch': t//self.param.epoch_len,
            'pi': self.policy.state_dict(),
            'q1': self.q1.state_dict(),
            'q2': self.q2.state_dict(),
            'replay_buffer': self.replay_buffer
        }, f"save/{self.env.get_name()}/checkpoint.pt")
