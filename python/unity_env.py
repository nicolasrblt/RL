from gymnasium.core import Env as GymEnv, ActType, ObsType
from typing import Any, SupportsFloat


class UnityEnv(GymEnv):
    """A gym compliant environment model representing unity env and communicating W: it through a socket"""

    def __init__(self) -> None:
        super().__init__()

    def reset():
        pass

    def step(self, action: ActType) -> tuple[ObsType, SupportsFloat, bool, bool, dict[str, Any]]:
        pass