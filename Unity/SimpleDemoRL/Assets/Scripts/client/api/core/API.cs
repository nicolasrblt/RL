using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class APIMessage{}  // TODO remove this, unnecessary


public struct ApiTuple {
    public Func<string> result;
    public Func<string, IEnumerator> runner;
    public ApiTuple(Func<string> res, Func<string, IEnumerator> run) {
        result = res;
        runner = run;
    }
}

public abstract class BaseApi<ArgType, RetType>
{
    protected RetType returnValue;

    public abstract IEnumerator Execute(string strArg);

    public string GetResult() {
        return returnValue != null ? JsonUtility.ToJson(returnValue) : null;
    }

    // two utility functions to help register api in manager.
    // Second one lets user instanciate API himself, allowing to give it parameters
    public static ApiTuple GetAPI<T>() where T: BaseApi<ArgType, RetType>, new()
    {
        T api = new T();
        return new ApiTuple(new Func<string>(api.GetResult), new Func<string, IEnumerator>(api.Execute));
    }

    public void Register(String name, APIManager manager)
    {
        manager.Register(name, new ApiTuple(new Func<string>(GetResult), new Func<string, IEnumerator>(Execute)));
    }
}


public abstract class Api<ArgType, RetType>: BaseApi<ArgType, RetType>
{
    public abstract RetType Handle(ArgType arg);

    public override IEnumerator Execute(string strArg) {
        ArgType arg = JsonUtility.FromJson<ArgType>(strArg);
        returnValue = Handle(arg);
        yield return null;
    }
}


public abstract class CoroutineApi<ArgType, RetType>: BaseApi<ArgType, RetType>
{
    public abstract IEnumerator Handle(ArgType arg);

    public override IEnumerator Execute(string strArg) {
        ArgType arg = JsonUtility.FromJson<ArgType>(strArg);
        yield return Handle(arg);
    }
}
