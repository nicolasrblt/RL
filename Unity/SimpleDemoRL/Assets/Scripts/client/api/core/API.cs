using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class APIMessage{}  // TODO remove this, unnecessary

public abstract class BaseAPI<ArgType, RetType>
{

    public abstract RetType Handle(ArgType arg);

    public string Execute(string strArg) {
        ArgType arg = JsonUtility.FromJson<ArgType>(strArg);
        RetType ret = Handle(arg);
        return ret != null ? JsonUtility.ToJson(ret) : null;
    }


    // two utility functions to help register api in manager.
    // Second one lets user instanciate API himself, allowing to give it parameters
    public static Func<string, string> GetAPI<T>() where T: BaseAPI<ArgType, RetType>, new()
    {
        return new Func<string, string>(new T().Execute);
    }

    public void Register(String name, APIManager manager)
    {
        manager.Register(name, new Func<string, string>(Execute));
    }
}
