


using UnityEngine;

/*
An Abstract class to represent a task completable by an agent
*/
public abstract class AgentTask
{
    /*
    Return The task description
    */
    public abstract string GetDisplayName();

    /*
    Computes the immediate reward for a state transition
    */
    public virtual float getReward(EnvState prevState, AgentAction action, EnvState currState)
    {
        return (isSuccess(currState)? 1:0) - (isFail(currState)?1:0);
    }

    /*
    Return a boolean indicating if the agent completed the task
    */
    public abstract bool isSuccess(EnvState state);

    /*
    Return a boolean indicating if the agent permanently failed the task, and needs reseting
    */
    public abstract bool isFail(EnvState state);
}