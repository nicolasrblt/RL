from dataclasses import dataclass

import numpy as np
import torch
from torch import nn
from torch.nn import functional



@dataclass
class TrainingParameters:
    """
    Dataclass holding SAC hyperparameters, see SACAgent.train() for more information
    """
    epochs: int = 100
    epoch_len: int = 4000
    start_steps: int = 10000
    update_after: int = 1000
    update_every: int = 50
    max_episode_len: int = 1000
    replay_size: int = int(1e6)
    batch_size: int = 100

    gamma: float = 0.99
    polyak: float = 0.995
    lr: float = 1e-3  # TODO : unused for now
    alpha: float = 0.2

    @property
    def steps(self):
        return self.epochs * self.epoch_len
    
    def update(self, new):
        for key, value in new.items():
            if hasattr(self, key):
                setattr(self, key, value)


def create_mlp(sizes, activation, output_activation=nn.Identity):
    """
    Creates a simple multilayer perceptron with `len(sizes)` layers, each containing `sizes[i]`  neurons
    sizes -- list of layer sizes
    activation -- activation function for all layers
    output_activation (optional) -- activation function for last layer
    """
    assert len(sizes) >= 2
    layers = []
    activations = [activation] * (len(sizes)-2) + [output_activation]
    for act, s1, s2 in zip(activations, sizes, sizes[1:]):
        layers += [nn.Linear(s1, s2), act()]
    return nn.Sequential(*layers)


class ReplayBuffer:
    """
    SAC Replay Buffer
    Holds a queue of interaction between an agent and its environment
    """
    def __init__(self, obs_dim, act_dim, size) -> None:
        self.max_size = size
        self.size = 0
        self.index = 0

        self.obs = np.zeros((size, obs_dim), dtype=np.float32)
        self.obs2 = np.zeros((size, obs_dim), dtype=np.float32)
        self.act = np.zeros((size, act_dim), dtype=np.float32)
        self.rew = np.zeros((size,), dtype=np.float32)
        self.done = np.zeros((size,), dtype=np.bool8)

    def record(self, obs, act, obs2, rew, done) -> None:
        """
        records an interaction or a set of interactions into the buffer
        """
        s = 1 if len(obs.shape) == 1 else obs.shape[0]
        ## TODO handle case where buffer needs to be written at its end *and* start (not critical if size is multiple of s)
        self.obs[self.index:self.index+s] = obs
        self.act[self.index:self.index+s] = act
        self.obs2[self.index:self.index+s] = obs2
        self.rew[self.index:self.index+s] = rew
        self.done[self.index:self.index+s] = done

        self.index = (self.index+s)%self.max_size
        if self.size < self.max_size:
            self.size = min(self.max_size, self.size+s)

    def sample(self, size: int) -> np.ndarray:
        """
        Samples randomly `size` interactions from the buffer and returns them as tensors
        """
        assert self.size > size
        index = np.random.randint(0, self.size, size)
        batch = dict(obs = self.obs[index],
                     act = self.act[index],
                     obs2 = self.obs2[index],
                     rew = self.rew[index],
                     done = self.done[index])
        return {k: torch.as_tensor(v, dtype=torch.float32) for k, v in batch.items()}
    sample_batch = sample  # for compatibility with team codebase

class QNet(nn.Module):
    """
    A class representing a Q-function as a feed-forward neural nerwork
    """
    def __init__(self, obs_dim, act_dim, hidden_sizes=(256, 256), activation=nn.ReLU) -> None:
        super().__init__()
        # use id as output activation func for qnets
        self.q = create_mlp([obs_dim+act_dim] + list(hidden_sizes) + [1], activation, nn.Identity)

    def forward(self, obs, act):  # -> value
        q = self.q(torch.cat((obs, act), -1))
        return torch.squeeze(q, -1)



LOG_STD_MAX = 2
LOG_STD_MIN = -20
class Policy(nn.Module):  # TODO rename Gaussian MLP ?
    """
    A class representing a policy as a gaussian feed-forward neural nerwork
    """
    def __init__(self, obs_dim, act_dim, act_high, hidden_sizes=(256, 256), activation=nn.ReLU) -> None:
        super().__init__()
        self.net = create_mlp([obs_dim]+list(hidden_sizes), activation, activation)  # last layers mu & std of policy net are computed separately
        self.mu_layer = nn.Linear(hidden_sizes[-1], act_dim)
        self.log_std_layer = nn.Linear(hidden_sizes[-1], act_dim)
        self.act_high = act_high


    def forward(self, obs, with_log_prob=True, probabilistic=True):  # -> action
        """
        Feed forward the network with inputs `obs`

        with_log_prob -- a boolean specifying if the log_likelihood of selected action should be returned as well.
            will raise an error if computed with non-probabilistic behaviour.
        probabilistic -- a boolean specifying if the policy should behave as probabilistic or deterministic. 
            If false, returns the distribution mean instead of sampling it.
        """
        assert probabilistic or not with_log_prob # no logprob on non stochastic action
        feat = self.net(obs)
        mu = self.mu_layer(feat)

        logp_pi = None

        if probabilistic:
            log_std = self.log_std_layer(feat)
            log_std = torch.clamp(log_std, LOG_STD_MIN, LOG_STD_MAX)
            std = torch.exp(log_std)
            distr = torch.distributions.normal.Normal(mu, std)
            action = distr.rsample()
        else:
            action = mu

        if with_log_prob:
            logp_pi = distr.log_prob(action).sum(axis=-1)
            logp_pi -= (2*(np.log(2) - action - functional.softplus(-2*action))).sum(axis=-1)
            ## formula from spinningup implementation, originally from arXiv 1801.01290 (appendix C)

        action = torch.tanh(action)  # normalize action to action space bounds
        action = self.act_high * action
        #if probabilistic and not with_log_prob:
        #    print(f"pi : {mu, std}")
        return action, logp_pi