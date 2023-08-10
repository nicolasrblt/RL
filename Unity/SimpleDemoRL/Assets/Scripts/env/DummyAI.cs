using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
A Dummy AI capable of controlling the agent to complete a specific task,
Used to collect samples of interactions
*/
public class DummyAI : MonoBehaviour
{
    public GoalManager goalManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public (float, float) StopInAI(Transform agentTransform, Transform targetTransform)
    {
        if (ComputeDistance(agentTransform, targetTransform) < 1f)
        {
            if(ComputeDistance(agentTransform, targetTransform) > 0.5f)
            {
                return (0.8f, 0f);
            }
            return (0f, 0f);
        }
        return GetInput(agentTransform, targetTransform);
    }

    public (float, float) PushInAI(Transform agentTransform, Transform ballTransform, Transform areaTransform)
    {
        if(ComputeDistance(agentTransform, ballTransform) > 0.8f)
        {
            return GetInput(agentTransform, ballTransform);
        }
        else
        {
            if(ComputeDistance(ballTransform, areaTransform) <= 0.5f)
            {
                return (0f, 0f);
            }
            return GetInput(agentTransform, areaTransform);
        }
    }

    public (float, float) PushOutAI(Transform agentTransform, Transform ballTransform, Transform areaTransform)
    {
        if (ComputeDistance(agentTransform, ballTransform) > 0.8f)
        {
            return GetInput(agentTransform, ballTransform);
        }

        if (ComputeDistance(ballTransform, areaTransform) > 1f)
        {
            return (0f, 0f);
        }

        return (1f, 0f);
    }

    public float ComputeAngle(Transform agentTransform, Transform targetTransform)
    {
        Vector3 targetDir = targetTransform.position - agentTransform.position;

        // Set the direction to be in the XZ plane only
        targetDir.y = 0;

        // Get the agent's forward direction in the XZ plane
        Vector3 agentForward = agentTransform.forward;
        agentForward.y = 0;

        // Calculate the angle between the agent's forward direction and the target direction
        float angle = Vector3.Angle(agentForward, targetDir);

        // Determine the sign of the angle based on the cross product of the vectors
        Vector3 cross = Vector3.Cross(agentForward, targetDir);
        if (cross.y < 0)
        {

            angle = -angle;
        }

        return angle;
    }

    public float ComputeDistance(Transform agentTransform, Transform targetTransform)
    {
        return Vector3.Distance(agentTransform.position, targetTransform.position);
    }

    public (float, float) GetInput(Transform agentTransform, Transform targetTransform)
    {
        float move = 0f;
        float turn = 0f;

        float angle = ComputeAngle(agentTransform, targetTransform);
        if (Mathf.Abs(angle) >= 10f)
        {
            if (angle > 0)
            {
                turn = 1f;
            }
            else if (angle < 0)
            {
                turn = -1f;
            }
            move = 0f;
        }
        else
        {
            turn = 0f;
            float distance = Vector3.Distance(agentTransform.position, targetTransform.position);
            move = Mathf.Min(distance, 1f);
        }

        return (move, turn);
    }
}
