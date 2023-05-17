import torch
import numpy as np

import sac

if __name__ == "__main__":
    import argparse
    import gymnasium as gym

    parser = argparse.ArgumentParser()
    parser.add_argument('--env', type=str, default='Pendulum-v1')
    parser.add_argument('--hid', type=int, default=256)
    parser.add_argument('--l', type=int, default=2)
    parser.add_argument('--gamma', type=float, default=0.99)  # discount factor
    parser.add_argument('--seed', '-s', type=int, default=0)
    parser.add_argument('--epochs', type=int, default=50)
    parser.add_argument('--max_step', type=int, default=None)
    parser.add_argument('--infer', action='store_true')
    parser.add_argument('--resume', action='store_true')
    args = parser.parse_args()

    torch.set_num_threads(torch.get_num_threads())

    seed = args.seed
    torch.manual_seed(seed)
    np.random.seed(seed)

    env = gym.make(args.env, render_mode='human' if args.infer else None, max_episode_steps=args.max_step)
    env = sac.GymEnv(env)
    env.action_space.seed(seed)
    agent = sac.SACAgent(env)


    if args.infer:
        cp = torch.load(f"save/{env.spec.id}/checkpoint.pt")
        agent.policy.load_state_dict(cp['pi'])
        agent.policy.eval()
        agent.test(float("+inf"))

    else:
        from_epoch = 0
        if args.resume:
            cp = torch.load(f"save/{env.spec.id}/checkpoint.pt")
            agent.policy.load_state_dict(cp['pi'])
            agent.q1.load_state_dict(cp['q1'])
            agent.q2.load_state_dict(cp['q2'])
            agent.replay_buffer = cp["replay_buffer"]
            from_epoch = cp['epoch']
        agent.train(seed, from_epoch=from_epoch, gamma=args.gamma, epochs=args.epochs)