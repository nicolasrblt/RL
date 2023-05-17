from gymnasium import Wrapper


class SACEnv:
    def step(self, action):
        raise NotImplemented

    def reset(self, seed=None):
        raise NotImplemented

    def pause(self):  # optional
        pass

    def resume(self):  # optional
        pass

    def get_obs_dim(self):
        raise NotImplemented

    def get_act_dim(self):
        raise NotImplemented

    def get_act_high(self):
        raise NotImplemented

    def get_random_act(self):
        raise NotImplemented

    def get_target_frame_duration(self):
        raise NotImplemented


class GymEnv(Wrapper, SACEnv):  # mro -> search first in gym wrapper then in SACEnv
    def get_obs_dim(self):
        return self.observation_space.shape[0]

    def get_act_dim(self):
        return self.action_space.shape[0]

    def get_act_high(self):
        return self.action_space.high[0] ## FIXME : doesnt support different highs in action dimensions

    def get_random_act(self):
        return self.action_space.sample()
    
    def get_target_frame_duration(self):
        return 0
