using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class APIManager
{
    private Dictionary<string, Func<string, string>> apiDictionary;

    public APIManager()
    {
        apiDictionary = new Dictionary<string, Func<string, string>>();
        
        //Register("test", new TestApi() as API);

        
        //Register("timeScale", new Func<string, string>((new TimeScaleAPI()).Execute));
        //Register("test", new Func<string, string>((new TestApi()).Execute));

        Register("test", TestApi.GetAPI<TestApi>());
        Register("timeScale", TimeScaleAPI.GetAPI<TimeScaleAPI>());
        Register("pause", PauseAPI.GetAPI<PauseAPI>());
        Register("step", StepAPI.GetAPI<StepAPI>());
        Register("reset", ResetAPI.GetAPI<ResetAPI>());
        
        foreach (KeyValuePair<string, Func<string, string>> kv in apiDictionary) {
            Debug.Log(kv.Key.ToString());
            Debug.Log(kv.Value.ToString());
        }
        
        Debug.Log("test exec timescale");
        string ret = apiDictionary["timeScale"]("{\"value\": 0.666}");
        Debug.Log($"test exec timescale ok, ret : {ret} ({ret==null})");

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
}
