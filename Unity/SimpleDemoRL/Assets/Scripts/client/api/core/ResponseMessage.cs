using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Message wrapping a response to the pyton side
*/
[System.Serializable]
public class ResponseMessage
{
    public string value;

    public static ResponseMessage FromJson(string json)
    {
        return JsonUtility.FromJson<ResponseMessage>(json);
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}
