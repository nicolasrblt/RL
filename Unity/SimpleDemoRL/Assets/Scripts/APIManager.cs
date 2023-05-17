using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APIManager
{
    private Dictionary<string, Action<string>> apiDictionary;

    public APIManager()
    {
        apiDictionary = new Dictionary<string, Action<string>>();
    }

    public void Register(string apiName, Action<string> api)
    {
        apiDictionary[apiName] = api;
    }

    public void Call(string apiName, string parameter)
    {
        if (apiDictionary.ContainsKey(apiName))
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => apiDictionary[apiName](parameter));
        }
    }
}
