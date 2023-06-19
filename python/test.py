import time
import random
import numpy as np

import messages
from server import Server
from agent import Agent
from unity_env import UnityEnv

try:
    server = Server('127.0.0.1', 5014)
    env = UnityEnv(server, 7, 2, 1, lambda n: 2*np.random.random((n, 2))-1, "SimpleUnityEnv", 49)
    agent = Agent(env, (256,)*6, (256)*6)

    env.start_server()

    env.set_time_scale(20)
    env.spawn_envs(env.get_agent_number())
    agent.train(from_epoch=0, epoch_len=10_000, start_steps=10_000, max_episode_len=3_000, update_every=50*env.get_agent_number())
finally:
    print("shutdown...")
    env.shutdown()
    server.stop_server()