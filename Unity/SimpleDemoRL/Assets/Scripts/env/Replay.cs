using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Replay
{
    public string instruction = "";
    public List<Step> steps = new List<Step>();

    public Replay(string instruction)
    {
        this.instruction = instruction;
    }

    public void AddStep(
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
        Step step = new Step(
            steps.Count,
            moveInputAction,
            turnInputAction,
            agentPosition,
            agentRotation,
            agentVelocity,
            agentAngularVelocity,
            redBallPosition,
            blueBallPosition,
            greenBallPosition,
            grayAreaPosition,
            orangeAreaPosition,
            whiteAreaPosition);

        steps.Add(step);
    }
}
