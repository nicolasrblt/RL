using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recorder : MonoBehaviour
{
    private Replay replay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetReplay(string instruction)
    {
        replay = new Replay(instruction);
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
        replay.AddStep(moveInputAction, turnInputAction,
            agentPosition, agentRotation, agentVelocity,
            agentAngularVelocity, redBallPosition, blueBallPosition,
            greenBallPosition, grayAreaPosition, orangeAreaPosition,
            whiteAreaPosition);
    }

    public int GetReplaySize()
    {
        return replay.steps.Count;
    }

    public List<Step> SampleBuffer()
    {
        List<Step> last64;

        if (replay.steps.Count < 64)
        {
            // If the list has less than 64 elements, pad it with the first element
            int paddingCount = 64 - replay.steps.Count;
            List<Step> padding = Enumerable.Repeat(replay.steps.First(), paddingCount).ToList();

            // Concatenate the padding and the original list
            last64 = padding.Concat(replay.steps).ToList();
        }
        else
        {
            // If the list has 64 or more elements, take the last 64
            last64 = replay.steps.TakeLast(64).ToList();
        }

        return last64;
    }

    public void SaveReplay(string fileName)
    {
        string json = JsonUtility.ToJson(replay);
        string path = Application.dataPath + "/data/";
        string file = path + fileName + ".json";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        System.IO.File.WriteAllText(file, json);
    }
}
