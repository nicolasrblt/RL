using UnityEngine;

[System.Serializable]

public class AgentAction
{
    public float moveInput;
    public float turnInput;


    public static AgentAction FromJson(string json)
    {
        return JsonUtility.FromJson<AgentAction>(json);
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}
