using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class APIMessage{}

public abstract class BaseAPI<ArgType, RetType> where ArgType: APIMessage where RetType: APIMessage
{

    public abstract RetType Handle(ArgType arg);

    public string Execute(string strArg) {  // FIXME put this in api manager instead ?
        ArgType arg = JsonUtility.FromJson<ArgType>(strArg);
        RetType ret = Handle(arg);
        return JsonUtility.ToJson(ret);
    }   
}
