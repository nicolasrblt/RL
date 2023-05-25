


using UnityEngine;

public abstract class AgentTask
{
    private string displayName = "No display name specified";
    public string getDisplayName() {
        return displayName;
    }
    public virtual float getReward(EnvState prevState, AgentAction action, EnvState currState)
    {
        Debug.Log("fuck");
        return (isSuccess(currState)? 1:0) - (isFail(currState)?1:0);
    }
    public abstract bool isSuccess(EnvState state);  // indicate if agent successfuly completed the task
    public abstract bool isFail(EnvState state);  // indicate if agent failed his task, and env needs reseting
}