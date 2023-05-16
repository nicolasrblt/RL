from copy import deepcopy
from itertools import chain
from dataclasses import dataclass
import os

import numpy as np
import torch
from torch.optim import Adam
from torch import nn

from sac_utils import TrainingParameters, create_mlp, QNet, Policy, ReplayBuffer


def get_obs_dim(env):
    return env.observation_space.shape[0]

def get_act_dim(env):
    return env.action_space.shape[0]

def get_act_high(env):
    return env.action_space.high[0] ## FIXME : doesnt support different highs in action dimensions

def get_random_act(env):
    return env.action_space.sample()

class SACAgent:
    def __init__(self, env) -> None:
        self.env = env
        self.policy = Policy(get_obs_dim(env), get_act_dim(env), get_act_high(env))
        self.q1 = QNet(get_obs_dim(env), get_act_dim(env))
        self.q2 = QNet(get_obs_dim(env), get_act_dim(env))
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
    def train(self, seed, from_epoch=0, **training_params):
        self.param.update(training_params)

        ### initialise stuff
        if self.replay_buffer is None:
            self.replay_buffer = ReplayBuffer(get_obs_dim(self.env), get_act_dim(self.env),
                                              self.param.replay_size)

        ###

        obs, *_ = self.env.reset(seed=seed)
        ep_len = 0

        assert not (any(torch.any(torch.isnan(p)) for p in self.policy.parameters()))

        print(from_epoch)
        for t in range(from_epoch*self.param.epoch_len, self.param.steps):
            assert not (any(torch.any(torch.isnan(p)) for p in self.policy.parameters()))

            if t > self.param.start_steps:
                act = self.act(obs)
            else:
                act = get_random_act(self.env)
            assert not (any(torch.any(torch.isnan(p)) for p in self.policy.parameters()))

            obs2, rew, terminated, truncated, *_ = self.env.step(act)
            done = terminated or truncated
            assert not (any(torch.any(torch.isnan(p)) for p in self.policy.parameters()))

            ep_len += 1
            if ep_len >= self.param.max_ep_len:
                done = True
            self.replay_buffer.record(obs, act, obs2, rew, done)
            obs = obs2

            assert not (any(torch.any(torch.isnan(p)) for p in self.policy.parameters()))

            if done: # reset env
                obs, *_ = self.env.reset()
                ep_len = 0

            if t % self.param.epoch_len == 0 and t >= 0:
                avg_rew, avg_len = self.test()
                print(f"=== epoch {t//self.param.epoch_len} || avg rew : {avg_rew} || avg len : {avg_len}")
                self.checkpoint(t)


            if t % self.param.update_every == 0 and t >= self.param.update_after:
                for _ in range(self.param.update_every):
                    batch = self.replay_buffer.sample(self.param.batch_size)

                    self.q1_optimizer.zero_grad()
                    self.q2_optimizer.zero_grad()
                    q1_loss, q2_loss = self.compute_loss_q(batch)
                    q1_loss.backward()
                    q2_loss.backward()
                    self.q1_optimizer.step()
                    self.q2_optimizer.step()

                    ## TODO surround this block by q.parms.require_grad=false ? and why ?
                    self.pi_optimizer.zero_grad()
                    pi_loss = self.compute_loss_pi(batch)
                    pi_loss.backward()
                    self.pi_optimizer.step()

                    with torch.no_grad():
                        for p, p_targ in zip (chain(self.q1.parameters(), self.q2.parameters()),
                                              chain(self.q1_targ.parameters(), self.q2_targ.parameters())):
                            p_targ.mul_(self.param.polyak)
                            p_targ.add_(p.mul(1-self.param.polyak))

                #print(f"after updt => t={t}, pi({act})")




    def act(self, obs): ## TODO rename this
        with torch.no_grad():
            a, _ = self.policy(torch.as_tensor(obs))
            return a.numpy()
        
    def compute_loss_pi(self, batch):
        obs = batch["obs"]
        act, logp = self.policy(obs)
        q_pi = torch.min(
            self.q1(obs, act),
            self.q2(obs, act)
        )
        assert not torch.isinf(torch.abs((q_pi-self.param.alpha*logp).mean()))
        assert not torch.isnan(torch.abs((q_pi-self.param.alpha*logp).mean()))
        return -(q_pi-self.param.alpha*logp).mean()
        # minus denotes the fact that we perform a gradient *ascent*

    def compute_loss_q(self, batch):
        obs, act, obs2, rew, done = batch["obs"], batch["act"], batch["obs2"], batch["rew"], batch["done"]
        
        with torch.no_grad():  # no grad while computing target
            pi_act, pi_logp = self.policy(obs2)

            q1_val = self.q1_targ(obs2, pi_act)
            q2_val = self.q2_targ(obs2, pi_act)
            min_q_pi = torch.min(
                q1_val,
                q2_val
            )
            target = rew + self.param.gamma * (1-done) * (min_q_pi-self.param.alpha*pi_logp)

        return ((self.q1(obs, act) - target)**2).mean(), ((self.q2(obs, act) - target)**2).mean()
    
    def test(self, max_ep=10):
        obs, *_ = self.env.reset()
        ep = 0
        ep_len = 0
        tot_rew = 0
        ep_ret = 0
        tot_len = 0

        while ep < max_ep:
            act = self.act(obs)
            obs, rew, terminated, truncated, *_ = self.env.step(act)
            done = terminated or truncated
            ep_len += 1
            tot_len += 1
            tot_rew += rew
            ep_ret += rew

            if done or ep_len >= self.param.max_ep_len:
                obs, *_ = self.env.reset()
                ep_len = 0
                ep_ret = 0
                ep += 1
        return tot_rew / max_ep, tot_len / max_ep


    def checkpoint(self, t):
        if not os.path.exists(f"save/{self.env.spec.id}/"):
            os.makedirs(f"save/{self.env.spec.id}/")
        torch.save({
            'epoch': t//self.param.epoch_len,
            'pi': self.policy.state_dict(),
            'q1': self.q1.state_dict(),
            'q2': self.q2.state_dict(),
            'replay_buffer': self.replay_buffer
        }, f"save/{self.env.spec.id}/checkpoint.pt")



if __name__ == "__main__":
    import argparse
    import gymnasium as gym

    parser = argparse.ArgumentParser()
    parser.add_argument('--env', type=str, default='Pendulum-v1')
    parser.add_argument('--hid', type=int, default=256)
    parser.add_argument('--l', type=int, default=2)
    parser.add_argument('--gamma', type=float, default=0.99)  # discount factor
    parser.add_argument('--seed', '-s', type=int, default=0)
    parser.add_argument('--epochs', type=int, default=50)
    parser.add_argument('--max_step', type=int, default=None)
    parser.add_argument('--infer', action='store_true')
    parser.add_argument('--resume', action='store_true')
    args = parser.parse_args()

    torch.set_num_threads(torch.get_num_threads())

    seed = args.seed
    torch.manual_seed(seed)
    np.random.seed(seed)

    env = gym.make(args.env, render_mode='human' if args.infer else None, max_episode_steps=args.max_step)
    env.action_space.seed(seed)
    agent = SACAgent(env)


    if args.infer:
        cp = torch.load(f"save/{env.spec.id}/checkpoint.pt")
        agent.policy.load_state_dict(cp['pi'])
        agent.policy.eval()
        agent.test(float("+inf"))

    else:
        from_epoch = 0
        if args.resume:
            cp = torch.load(f"save/{env.spec.id}/checkpoint.pt")
            agent.policy.load_state_dict(cp['pi'])
            agent.q1.load_state_dict(cp['q1'])
            agent.q2.load_state_dict(cp['q2'])
            agent.replay_buffer = cp["replay_buffer"]
            from_epoch = cp['epoch']
        agent.train(seed, from_epoch=from_epoch, gamma=args.gamma, epochs=args.epochs)
        torch.save(agent.policy.state_dict("pi.pt"))


    
    print("done.")
