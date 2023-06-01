


using UnityEngine;

public abstract class AgentTask
{
    public abstract string GetDisplayName();
    public virtual float getReward(EnvState prevState, AgentAction action, EnvState currState)
    {
        return (isSuccess(currState)? 1:0) - (isFail(currState)?1:0);
    }
    public abstract bool isSuccess(EnvState state);  // indicate if agent successfuly completed the task
    public abstract bool isFail(EnvState state);  // indicate if agent failed his task, and env needs reseting
}