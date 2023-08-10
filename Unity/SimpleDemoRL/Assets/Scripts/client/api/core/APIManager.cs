using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
A manager to register and call APIs
Extend this class and override RegisterAllApis to register user APIs
*/
public abstract class APIManager
{
    protected Dictionary<string, ApiFacade> apiDictionary;

    public APIManager()
    {
        apiDictionary = new Dictionary<string, ApiFacade>();
    }

    public void Register(string apiName, ApiFacade api)
    {
        apiDictionary[apiName] = api;
    }

    public ApiFacade GetApi(string apiName)
    {
        if (apiDictionary.ContainsKey(apiName))
        {
            return apiDictionary[apiName];
        }
        Debug.LogWarning($"APIManager : WARNING : {apiName} api not found");
        return new ApiFacade();
    }

    public abstract void RegisterAllApis();
}
