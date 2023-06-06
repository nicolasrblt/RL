from gymnasium.core import Env as GymEnv, ActType, ObsType
from typing import Any, SupportsFloat
import torch

from sac import SACEnv
import messages
from server import Server


def dict_to_vector(x, dims, norm=1):
    vector = []
    for dim in dims:
        vector.append(x[dim]/norm)
        
    return vector

def pharse_observations(obs):  ## TODO : rename 'obs_to_vect' and change return accordingly
    vector = []
    vector += dict_to_vector(obs.agentPostion, ["x", "z"], 10)
    vector += dict_to_vector(obs.agentRotation, ["y"], 360)
    vector += dict_to_vector(obs.velocity, ["x", "z"], 10)
    #vector += dict_to_vector(obs.redBallPosition, ["x", "z"], 10)
    #vector += dict_to_vector(obs.grayAreaPosition, ["x", "z"], 10)
    
    #vector += [obs.agentRedBallAngle / 360]
    vector += [obs.agentGrayAreaAngle / 360]
    #vector += [obs.agentRedBallDist / 30]
    vector += [obs.agentGrayAreaDist / 30]
    #vector += [obs.RedBallGreyAreaDist / 30]
    
    done = obs.terminate
    reward = obs.reward

    return torch.Tensor(vector), reward, done  ## FIXME(ok) : remove brackets around vector FIXME : should return array not tensor


class UnityEnv(SACEnv):
    """A gym compliant environment model representing unity env and communicating W: it through a socket"""

    def __init__(self, server, obs_dim, act_dim, act_high, sample_act_func, name) -> None:
        super().__init__()
        self.server = server
        self.obs_dim = obs_dim
        self.act_dim = act_dim
        self.act_high = act_high
        self.sample_act_func = sample_act_func
        self.name = name
        self.time_scale = 1

    def start_server(self):
        self.server.start_server()
 
    def shutdown(self):
        message = messages.RequestMessage("shutdown", "")
        self.server.send([message.to_json()])

    def reset(self, *, seed: int | None = None, options: dict[str, Any] | None = None,) -> tuple[ObsType, dict[str, Any]]:
        ## FIXME : reset seems to also return a reward and done flag : remove this
        message = messages.RequestMessage("reset", "")
        
        self.server.send([message.to_json()])
        
        response = messages.ResponseMessage.from_json(self.server.recive()[0])
        observation_message = messages.ObservationMessage.from_json(response.value)
        
        return pharse_observations(observation_message)
    
    def step(self, action: ActType) -> tuple[ObsType, SupportsFloat, bool, bool, dict[str, Any]]:
        controll_message = messages.ControllMessage(action)
        message = messages.RequestMessage("step", controll_message.to_json())
        
        self.server.send([message.to_json()])
        
        response = messages.ResponseMessage.from_json(self.server.recive()[0])
        observation_message = messages.ObservationMessage.from_json(response.value)
        
        return *pharse_observations(observation_message), False  ## TODO : replace false by truncated actual value

    def get_target_frame_duration(self):
        return 0
    
    def get_obs_dim(self):
        return self.obs_dim
    
    def get_act_dim(self):
        return self.act_dim

    def get_act_high(self):
        return self.act_high ## FIXME : doesnt support different highs in action dimensions

    def get_random_act(self):
        return self.sample_act_func()

    def pause(self):
        message = messages.RequestMessage("pause", messages.SingleFieldMessage(True).to_json())
        self.server.send([message.to_json()])

    def resume(self):
        message = messages.RequestMessage("pause", messages.SingleFieldMessage(False).to_json())
        self.server.send([message.to_json()])

    def get_name(self):
        return self.name
    
    def set_time_scale(self, ts):
        self.time_scale = ts
        ts_message = messages.SingleFieldMessage(ts)
        message = messages.RequestMessage("timeScale", ts_message.to_json())
        self.server.send([message.to_json()])