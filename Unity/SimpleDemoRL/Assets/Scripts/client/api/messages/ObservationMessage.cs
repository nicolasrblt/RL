using UnityEngine;

[System.Serializable]
public class ObservationMessage
{
    public Vector3 agentPostion;
    public Vector3 agentRotation;
    public Vector3 velocity;
    public Vector3 angularVelocity;
    public Vector3 redBallPosition;
    public Vector3 blueBallPosition;
    public Vector3 greenBallPosition;
    public Vector3 grayAreaPosition;
    public Vector3 orangeAreaPosition;
    public Vector3 whiteAreaPosition;
    public bool terminate;
    public float reward;


    public static ObservationMessage FromJson(string json)
    {
        return JsonUtility.FromJson<ObservationMessage>(json);
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}