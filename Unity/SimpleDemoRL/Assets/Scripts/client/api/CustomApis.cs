using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseEnvManagerAPI<ArgType, RetType>: BaseAPI<ArgType, RetType>
{
    protected EnvorinmentManager env;
}


public class TimeScaleAPI: BaseAPI<SingleFieldMessage<float>, string>
{
    public override string Handle(SingleFieldMessage<float> msg) {
        Debug.Log($"time scale : {(float)msg}");
        return null;
    }
}

public class StepAPI: BaseAPI<ControllMessage, ObservationMessage>
{
    public override ObservationMessage Handle(ControllMessage msg) {
        Debug.Log($"control : {msg.moveInput}, {msg.turnInput}");
        return null;
    }
}

public class ResetAPI: BaseAPI<string, ObservationMessage>
{
    public override ObservationMessage Handle(string msg) {
        Debug.Log($"reset");
        return null;
    }
}

public class PauseAPI: BaseAPI<SingleFieldMessage<bool>, string>
{
    public override string Handle(SingleFieldMessage<bool> msg) {
        Debug.Log($"pause : {(bool)msg}");
        return null;
    }
}


// Check and Shutdown