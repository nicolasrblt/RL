import time
import random
import numpy as np

import messages
from simulator import Simulator
from agent import Agent
from unity_env import UnityEnv

try:
    simulator = Simulator(port=5004)
    env = UnityEnv(simulator, 9, 2, 1, lambda: np.random.random((2,)), "SimpleDemoEnv")
    agent = Agent(env)

    simulator.start_server()

    input("start unity env, then press enter")
    agent.train()
finally:
    print("shutdown...")
    simulator.shutdown()
    simulator.server.stop_server()