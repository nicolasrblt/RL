from gymnasium import Wrapper


class SACEnv:
    """
    An abstract class specifying the environment API. compliant with OpenAI gym API's core features
    """
    def step(self, action):
        raise NotImplemented

    def reset(self, done=True):
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
    
    def get_name(self):
        raise NotImplemented
    
    def get_agent_number(self):
        return 1


class GymEnv(Wrapper, SACEnv):  # mro -> search first in gym wrapper then in SACEnv
    """
    A wrapper around a Gym environment to make it compliant with `SACEnv` interface
    """
    def reset(self, done=True, **kwargs):
        if done:
            super().reset(**kwargs)
            
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
    
    def get_name(self):
        return self.spec.id
