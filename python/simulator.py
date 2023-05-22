import json
import time
import torch
import subprocess
import messages
from server import Server


def dict_to_vector(x, dims, norm=1):
    vector = []
    for dim in dims:
        vector.append(x[dim]/norm)
        
    return vector

def pharse_observations(obs):
    vector = []
    vector += dict_to_vector(obs.agentPostion, ["x", "z"], 10)
    vector += dict_to_vector(obs.agentRotation, ["y"], 360)
    vector += dict_to_vector(obs.velocity, ["x", "z"], 10)
    vector += dict_to_vector(obs.redBallPosition, ["x", "z"], 10)
    #vector += dict_to_vector(obs.blueBallPosition, ["x", "z"])
    #vector += dict_to_vector(obs.greenBallPosition, ["x", "z"])
    vector += dict_to_vector(obs.grayAreaPosition, ["x", "z"], 10)
    #vector += dict_to_vector(obs.orangeAreaPosition, ["x", "z"])
    #vector += dict_to_vector(obs.whiteAreaPosition, ["x", "z"])
    
    done = obs.terminate
    reward = obs.reward

    
    return torch.Tensor(vector), reward, done  ## FIXME(ok) : remove brackets around vector FIXME : should return array not tensor

class Simulator:
    def __init__(self, host="127.0.0.1", port=5004) -> None:
        self.server = Server(host, port)
        
    def start_server(self):
        self.server.start_server()
        
    def reset(self):
        message = messages.RequestMessage("reset", "")
        
        self.server.send([message.to_json()])
        
        response = messages.ResponseMessage.from_json(self.server.recive()[0])
        observation_message = messages.ObservationMessage.from_json(response.value)
        
        return pharse_observations(observation_message)
        
    
    def step(self, moveInput, turnInput):
        controll_message = messages.ControllMessage(moveInput, turnInput)
        message = messages.RequestMessage("step", controll_message.to_json())
        
        self.server.send([message.to_json()])
        
        response = messages.ResponseMessage.from_json(self.server.recive()[0])
        observation_message = messages.ObservationMessage.from_json(response.value)
            
        return pharse_observations(observation_message)
    
    def pause(self):
        message = messages.RequestMessage("pause", "pause")
        
        self.server.send([message.to_json()])
        
    def resume(self):
        message = messages.RequestMessage("pause", "resume")
        
        self.server.send([message.to_json()])
        
    def shutdown(self):
        message = messages.RequestMessage("shutdown", "")
        
        self.server.send([message.to_json()])
        
    def set_time_scale(self, ts):
        ts_message = messages.SingleFieldMessage(ts)
        message = messages.RequestMessage("timeScale", ts_message.to_json())
        self.server.send([message.to_json()])
    
