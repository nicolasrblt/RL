using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestApi:BaseAPI<APIMessage, APIMessage>
{
    public override APIMessage Handle(APIMessage arg) {
        Debug.Log("test api : "+arg);
        return null;
    }
}
