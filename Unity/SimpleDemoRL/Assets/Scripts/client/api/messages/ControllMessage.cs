using UnityEngine;

[System.Serializable]

public class ControllMessage
{
    public float moveInput;
    public float turnInput;


    public static ControllMessage FromJson(string json)
    {
        return JsonUtility.FromJson<ControllMessage>(json);
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}
