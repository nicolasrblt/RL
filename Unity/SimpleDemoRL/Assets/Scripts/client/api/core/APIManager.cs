using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class APIManager
{
    private Dictionary<string, Func<string, string>> apiDictionary;

    public APIManager()
    {
        apiDictionary = new Dictionary<string, Func<string, string>>();
    }

    public void Register(string apiName, Func<string, string> api)
    {
        apiDictionary[apiName] = api;
    }

    public string Call(string apiName, string parameter)
    {
        if (apiDictionary.ContainsKey(apiName))  // TODO try removing multithreading
        {
            Debug.Log($"APIManager : found {apiName}, calling it...");
            //UnityMainThreadDispatcher.Instance().Enqueue(() => apiDictionary[apiName].Execute(parameter));
            return apiDictionary[apiName](parameter);
        }
        Debug.Log($"APIManager : WARNING : {apiName} api not found");
        return "";
    }

    public abstract void RegisterAllApis();
}
