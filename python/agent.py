from sac import SACAgent


class Agent(SACAgent):
    def get_action(self, obs, probabilistic=True):
        act = super().get_action(obs, probabilistic)
        return act