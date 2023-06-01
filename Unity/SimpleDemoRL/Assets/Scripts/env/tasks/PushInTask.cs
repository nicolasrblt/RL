


using UnityEngine;

public class PushInTask: AgentTask
{    public override float getReward(EnvState prevState, AgentAction action, EnvState currState)
    {
        if (isSuccess(currState)) {
            return 50;
        }
        if (isFail(currState)) {
            return -50;
        }
        float currDistanceBallArea = Vector3.Distance(currState.redBallPosition, currState.grayAreaPosition);
        float prevDistanceBallArea = Vector3.Distance(prevState.redBallPosition, prevState.grayAreaPosition);

        float currDistanceBallAgent = Vector3.Distance(currState.redBallPosition, currState.agentPostion);
        float prevDistanceBallAgent = Vector3.Distance(prevState.redBallPosition, prevState.agentPostion);

        return (currDistanceBallAgent/30.0f-1 + currDistanceBallArea/30.0f-1 - currState.agentRedBallAngle/180.0f)/30.0f; // [-0.1, 0]
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