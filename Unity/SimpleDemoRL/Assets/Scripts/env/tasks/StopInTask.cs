using UnityEngine;

public class StopInTask: AgentTask
{    public override float getReward(EnvState prevState, AgentAction action, EnvState currState)
    {
        if (isSuccess(currState)) {
            return 100;
        }
        if (isFail(currState)) {
            return -0;
        }
        //float currDist = Vector3.Distance(currState.agentPostion, currState.grayAreaPosition);
        //float prevDist = Vector3.Distance(prevState.agentPostion, prevState.grayAreaPosition);

        return (-currState.agentGrayAreaDist/30.0f - currState.agentGrayAreaAngle/180.0f)/20.0f;  // [-0.1; 0]
        //return prevDist - currDist;
    }

    public override bool isSuccess(EnvState state)
    {
        return state.agentGrayAreaDist <= 0.5;
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