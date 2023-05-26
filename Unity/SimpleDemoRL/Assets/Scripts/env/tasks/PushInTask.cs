


using UnityEngine;

public class PushInTask: AgentTask
{    public override float getReward(EnvState prevState, AgentAction action, EnvState currState)
    {
        if (isSuccess(currState)) {
            return 20;
        }
        if (isFail(currState)) {
            return -5;
        }
        float currDistanceBallArea = Vector3.Distance(currState.redBallPosition, currState.grayAreaPosition);
        float prevDistanceBallArea = Vector3.Distance(prevState.redBallPosition, prevState.grayAreaPosition);

        float currDistanceBallAgent = Vector3.Distance(currState.redBallPosition, currState.agentPostion);
        float prevDistanceBallAgent = Vector3.Distance(prevState.redBallPosition, prevState.agentPostion);

        return 1*(prevDistanceBallArea-currDistanceBallArea)  +  2*(prevDistanceBallAgent-currDistanceBallAgent);
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

    void Awake()
    {
        displayName = "Push the red ball in the gray area";
    }
}