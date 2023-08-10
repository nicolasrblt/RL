from sac import SACAgent


class Agent(SACAgent):
    """
    A wrapper of SACAgent
    representation of an agent compatible with UnityEnvironment
    """
    def get_action(self, obs, probabilistic=True):
        act = super().get_action(obs, probabilistic)
        return act