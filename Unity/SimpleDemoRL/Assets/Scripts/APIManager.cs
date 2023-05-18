using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using API = BaseAPI<APIMessage, APIMessage>;

public class APIManager
{
    private Dictionary<string, API> apiDictionary;

    public APIManager()
    {
        apiDictionary = new Dictionary<string, API>();
        
        Register("test", new TestApi());
    }

    public void Register(string apiName, API api)
    {
        apiDictionary[apiName] = api;
    }

    public string Call(string apiName, string parameter)
    {
        if (apiDictionary.ContainsKey(apiName))  // TODO try removing multithreading
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => apiDictionary[apiName].Execute(parameter));
            return apiDictionary[apiName].Execute(parameter);
        }
        return "";
    }
}
