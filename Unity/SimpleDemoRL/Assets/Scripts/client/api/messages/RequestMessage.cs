using UnityEngine;

[System.Serializable]
public class RequestMessage
{
    public string api;
    public string parameter;

    public T GetParameter<T>()
    {
        return JsonUtility.FromJson<T>(parameter);
    }

    public void SetParameter<T>(T value)
    {
        parameter = JsonUtility.ToJson(value);
    }

    public static RequestMessage FromJson(string json)
    {
        return JsonUtility.FromJson<RequestMessage>(json);
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}
