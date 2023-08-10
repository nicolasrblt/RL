using UnityEngine;
using System;


/*
A Utility classs to send an optional message
*/
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
