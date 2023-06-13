import time
import random
import numpy as np

import messages
from server import Server
from agent import Agent
from unity_env import UnityEnv

try:
    server = Server('127.0.0.1', 5006)
    env = UnityEnv(server, 7, 2, 1, lambda n: 2*np.random.random((n, 2))-1, "SimpleUnityEnv", 2)
    agent = Agent(env, (256,)*6, (256)*6)

    env.start_server()

    env.set_time_scale(20)
    agent.train(from_epoch=0, epoch_len=10_000, start_steps=10_000, max_episode_len=4_000)
finally:
    print("shutdown...")
    env.shutdown()
    server.stop_server()