using UnityEngine;

public class StopInTask: AgentTask
{    public override float getReward(EnvState prevState, AgentAction action, EnvState currState)
    {
        if (isSuccess(currState)) {
            return 20;
        }
        if (isFail(currState)) {
            return -5;
        }
        float currDist = Vector3.Distance(currState.agentPostion, currState.grayAreaPosition);
        float prevDist = Vector3.Distance(prevState.agentPostion, prevState.grayAreaPosition);

        //return currDist/40-1;
        return prevDist - currDist;
    }

    public override bool isSuccess(EnvState state)
    {
        float distance = Vector3.Distance(state.redBallPosition, state.grayAreaPosition);
        return distance <= 0.5;
    }

    public override bool isFail(EnvState state)
    {
        return state.agentOutsidePlane || state.redBallOutsidePlane;
    }

    public override string GetDisplayName()
    {
        return "Stop the agent in " + "gray" + " area";
    }
}