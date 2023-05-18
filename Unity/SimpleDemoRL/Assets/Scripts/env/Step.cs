using UnityEngine;

[System.Serializable]
public class Step
{
    public int stepNumber;
    public float moveInputAction;
    public float turnInputAction;
    public Vector3 agentPosition;
    public Vector3 agentRotation;
    public Vector3 agentVelocity;
    public Vector3 agentAngularVelocity;

    public Vector3 redBallPosition;
    public Vector3 blueBallPosition;
    public Vector3 greenBallPosition;

    public Vector3 grayAreaPosition;
    public Vector3 orangeAreaPosition;
    public Vector3 whiteAreaPosition;

    public Step(
        int stepNumber,
        float moveInputAction,
        float turnInputAction,
        Vector3 agentPosition,
        Vector3 agentRotation,
        Vector3 agentVelocity,
        Vector3 agentAngularVelocity,
        Vector3 redBallPosition,
        Vector3 blueBallPosition,
        Vector3 greenBallPosition,
        Vector3 grayAreaPosition,
        Vector3 orangeAreaPosition,
        Vector3 whiteAreaPosition)
    {
        this.stepNumber = stepNumber;
        this.moveInputAction = moveInputAction;
        this.turnInputAction = turnInputAction;
        this.agentPosition = agentPosition;
        this.agentRotation = agentRotation;
        this.agentVelocity = agentVelocity;
        this.agentAngularVelocity = agentAngularVelocity;

        this.redBallPosition = redBallPosition;
        this.blueBallPosition = blueBallPosition;
        this.greenBallPosition = greenBallPosition;

        this.grayAreaPosition = grayAreaPosition;
        this.orangeAreaPosition = orangeAreaPosition;
        this.whiteAreaPosition = whiteAreaPosition;
    }
}
