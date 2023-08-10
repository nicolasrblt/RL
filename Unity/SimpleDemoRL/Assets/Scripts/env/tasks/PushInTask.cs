


using UnityEngine;

/*
A Task consisting in pushing a ball to a target area
*/
public class PushInTask: AgentTask
{    public override float getReward(EnvState prevState, AgentAction action, EnvState currState)
    {
        if (isSuccess(currState)) {
            return 100;
        }
        if (isFail(currState)) {
            return 0;
        }
        float currDistanceBallArea = Vector3.Distance(currState.redBallPosition, currState.grayAreaPosition);
        float prevDistanceBallArea = Vector3.Distance(prevState.redBallPosition, prevState.grayAreaPosition);

        float currDistanceBallAgent = Vector3.Distance(currState.redBallPosition, currState.agentPostion);
        float prevDistanceBallAgent = Vector3.Distance(prevState.redBallPosition, prevState.agentPostion);

        return (-currDistanceBallAgent/30.0f -currDistanceBallArea/30.0f - currState.agentRedBallAngle/180.0f)/30.0f; // [-0.1, 0]
        //return 1*(prevDistanceBallArea-currDistanceBallArea)  +  2*(prevDistanceBallAgent-currDistanceBallAgent);
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
        return "Push the " + "red ball" + " in " + "gray" + " area";
    }
}