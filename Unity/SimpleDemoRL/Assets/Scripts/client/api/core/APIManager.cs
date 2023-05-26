using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class APIManager
{
    private Dictionary<string, ApiTuple> apiDictionary;

    public APIManager()
    {
        apiDictionary = new Dictionary<string, ApiTuple>();
    }

    public void Register(string apiName, ApiTuple api)
    {
        apiDictionary[apiName] = api;
    }

    public ApiTuple GetApi(string apiName)
    {
        if (apiDictionary.ContainsKey(apiName))
        {
            return apiDictionary[apiName];
        }
        Debug.LogWarning($"APIManager : WARNING : {apiName} api not found");
        return new ApiTuple();
    }

    public abstract void RegisterAllApis();
}
