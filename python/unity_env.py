from gymnasium.core import Env as GymEnv, ActType, ObsType
from typing import Any
import torch
import numpy as np

from sac import SACEnv
import messages
from server import Server


def dict_to_vector(x, dims, norm=1):
    """
    Turn a dict to a list selecting certain items

    dims -- items to select
    """
    vector = []
    for dim in dims:
        vector.append(x[dim]/norm)
        
    return vector

def obs_to_vect(obs):
    """
    Turn an observation object into a vector selecting only desired features

    return --> a vector containing selected features
    """
    vector = []
    vector += dict_to_vector(obs.agentPostion, ["x", "z"], 10)
    vector += dict_to_vector(obs.agentRotation, ["y"], 360)
    vector += dict_to_vector(obs.velocity, ["x", "z"], 10)
    
    vector += [obs.agentGrayAreaAngle / 360]
    vector += [obs.agentGrayAreaDist / 30]

    return vector  #torch.Tensor(vector), reward, done


class UnityEnv(SACEnv):
    """An Unity Environment model compliant with OpenAI gym API"""

    def __init__(self, server: Server, obs_dim, act_dim, act_high, sample_act_func, name, agent_number=1) -> None:
        super().__init__()
        self.server = server
        self.obs_dim = obs_dim
        self.act_dim = act_dim
        self.act_high = act_high
        self.sample_act_func = sample_act_func
        self.name = name
        self.agent_number = agent_number
        self.time_scale = 1

    def start_server(self):
        self.server.start_server()
 
    def shutdown(self):
        """Shutdown the foreign Unity environment"""
        message = messages.RequestMessage("shutdown", "")
        self.server.send(message.to_json())

    def reset(self, envDone=None) -> list[ObsType] | ObsType:
        """
        Reset the environment

        envDone (optional) -- list of id of ennvironments to reset if working with multiple agents
        return -> observation for environment or list of observation for multiple agent after reset
        """
        if isinstance(envDone, int):
            req_json = get_json_request("reset", messages.SingleFieldMessage(envDone))
            self.server.send(req_json)

            resp_json = self.server.receive()
            observation_message = get_response_msg(resp_json, messages.ObservationMessage)

            return (np.array(obs_to_vect(observation_message), dtype=np.float32),)
        
        else:
            if envDone is None:
                m = np.arange(self.get_agent_number())
            else:
                m =  np.where(envDone)[0]
            req_json = get_json_request("multiReset", messages.MultiMessage(m.tolist()))
            self.server.send(req_json)

            resp_json = self.server.receive()
            multi_obs_msg = get_response_msg(resp_json, messages.MultiObservationMessage)

            return (np.array([obs_to_vect(obs) for obs in multi_obs_msg.messages], dtype=np.float32),)
    
    def step(self, action: list[ActType] | ActType, envNum: int | list[int]=0) -> tuple[list[ObsType] | ObsType, float, bool, bool]:
        """
        Perform one step in the environment

        action -- action or list of action to perform
        envNums (optional) -- id of agent to step if stepping a single one, by default 0.

        return -> observation for environment or list of observation for multiple agent after step
        """
        if len(np.shape(action)) == 2:
            actions = [messages.ControllMessage(*a, i) for (i, a) in enumerate(action)]
            req_json = get_json_request("multiStep", messages.MultiMessage(actions))
            self.server.send(req_json)
            
            resp_json = self.server.receive()
            multi_obs_msg = get_response_msg(resp_json, messages.MultiObservationMessage)
            
            return (
                np.array([obs_to_vect(obs) for obs in multi_obs_msg.messages], dtype=np.float32),
                np.array([obs.reward for obs in multi_obs_msg.messages], dtype=np.float32),
                np.array([obs.terminate for obs in multi_obs_msg.messages]),
                np.array([False for obs in multi_obs_msg.messages])  ## TODO : replace false by truncated actual value
            )
        
        else:  # TODO : use get_json_request and get_response_msg
            controll_message = messages.ControllMessage(*action, envNum=envNum)
            message = messages.RequestMessage("step", controll_message.to_json())
            
            self.server.send(message.to_json())
            
            response = messages.ResponseMessage.from_json(self.server.receive())
            observation_message = messages.ObservationMessage.from_json(response.value)
            
            return np.array(obs_to_vect(observation_message), dtype=np.float32), observation_message.reward, observation_message.terminate, False  ## TODO : replace false by truncated actual value

    def get_target_frame_duration(self):
        """
        Return nominal frame duration of the environment
        """
        return 0
    
    def get_obs_dim(self):
        return self.obs_dim
    
    def get_act_dim(self):
        return self.act_dim

    def get_act_high(self):
        return self.act_high

    def get_random_act(self):
        return self.sample_act_func(self.get_agent_number())

    def pause(self):
        """
        Pause the foreign Unity environment
        """
        message = messages.RequestMessage("pause", messages.SingleFieldMessage(True).to_json())
        self.server.send(message.to_json())

    def resume(self):
        """
        Resume the foreign Unity environment
        """
        message = messages.RequestMessage("pause", messages.SingleFieldMessage(False).to_json())
        self.server.send(message.to_json())

    def get_name(self):
        """
        Return unique name of the instanced environment
        """
        return self.name
    
    def get_agent_number(self):
        """
        Return number of agent in the environment
        """
        return self.agent_number
    
    def set_time_scale(self, ts: int):
        """
        Set `timeScale` property of the Unity environment
        """
        self.time_scale = ts
        req_json = get_json_request("timeScale", messages.SingleFieldMessage(ts))
        self.server.send(req_json)

    def spawn_envs(self, n):
        """
        Spawn multiple copy of an environment in the same scene

        n -- number of copies to spawn
        """
        req_json = get_json_request("spawnEnvs", messages.SingleFieldMessage(n))
        self.server.send(req_json)

## internal functions

def get_json_request(api, message):
    return messages.RequestMessage(api, message.to_json()).to_json()

def get_response_msg(resp_json, msg_cls):
    response = messages.ResponseMessage.from_json(resp_json)
    return msg_cls.from_json(response.value)
