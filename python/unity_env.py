from gymnasium.core import Env as GymEnv, ActType, ObsType
from typing import Any, SupportsFloat

from sac import SACEnv


class UnityEnv(SACEnv):
    """A gym compliant environment model representing unity env and communicating W: it through a socket"""

    def __init__(self, communicator, obs_dim, act_dim, act_high, sample_act_func, name) -> None:
        super().__init__()
        self.communicator = communicator
        self.obs_dim = obs_dim
        self.act_dim = act_dim
        self.act_high = act_high
        self.sample_act_func = sample_act_func
        self.name = name
        self.time_scale = 1

    def reset(self, *, seed: int | None = None, options: dict[str, Any] | None = None,) -> tuple[ObsType, dict[str, Any]]:
        return self.communicator.reset()  ## FIXME : reset seems to also return a reward and done flag : remove this

    def step(self, action: ActType) -> tuple[ObsType, SupportsFloat, bool, bool, dict[str, Any]]:
        o, r, d = self.communicator.step(*action)  ## TODO : unpack args here to be compatible w/ 'legacy' communicator, make it general purpose later
        return o, r, d, False  ## TODO : replace false by truncated actual value

    def get_target_frame_duration(self):
        return (1/50)/self.time_scale
    
    def get_obs_dim(self):
        return self.obs_dim
    
    def get_act_dim(self):
        return self.act_dim

    def get_act_high(self):
        return self.act_high ## FIXME : doesnt support different highs in action dimensions

    def get_random_act(self):
        return self.sample_act_func()

    def pause(self):
        self.communicator.pause()

    def resume(self):
        self.communicator.resume()

    def get_name(self):
        return self.name
    
    def set_time_scale(self, ts):
        self.time_scale = ts
        self.communicator.set_time_scale(ts)
