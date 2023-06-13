using UnityEngine;

[System.Serializable]
public class EnvState
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
    public float agentRedBallAngle;
    public float agentGrayAreaAngle;
    public float agentRedBallDist;
    public float agentGrayAreaDist;
    public float RedBallGreyAreaDist;
    public bool agentOutsidePlane;
    public bool redBallOutsidePlane;
    public bool terminate;
    public float reward;
    public int envNum;


    public static EnvState FromJson(string json)
    {
        return JsonUtility.FromJson<EnvState>(json);
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}
