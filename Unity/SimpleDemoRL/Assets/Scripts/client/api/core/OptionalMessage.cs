using UnityEngine;
using System;

[System.Serializable]
public class OptionalMessage<T> where T : class
{
    public static T FromJson(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }
        return JsonUtility.FromJson<T>(json);
    }
}
