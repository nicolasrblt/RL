using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseEnvManagerAPI<ArgType, RetType>: BaseAPI<ArgType, RetType>
{
    protected EnvorinmentManager env;

    public BaseEnvManagerAPI(EnvorinmentManager target): base()
    {
        env = target;
    }
}


public class TimeScaleAPI: BaseEnvManagerAPI<SingleFieldMessage<float>, string>
{
    public TimeScaleAPI(EnvorinmentManager env): base(env) {}
    public override string Handle(SingleFieldMessage<float> msg) {
        Debug.Log($"time scale : {(float)msg}");
        env.SetTimeScale(msg);
        return null;
    }
}

public class StepAPI: BaseEnvManagerAPI<ControllMessage, ObservationMessage>
{
    public StepAPI(EnvorinmentManager env): base(env) {}
    public override ObservationMessage Handle(ControllMessage msg) {
        Debug.Log($"control : {msg.moveInput}, {msg.turnInput}");
        env.Step(msg);
        return env.GetObservation();
    }
}

public class ResetAPI: BaseEnvManagerAPI<string, ObservationMessage>
{

    public ResetAPI(EnvorinmentManager env): base(env) {}
    public override ObservationMessage Handle(string msg) {
        Debug.Log($"reset");
        env.Reset();
        return env.GetObservation();
    }
}

public class PauseAPI: BaseEnvManagerAPI<SingleFieldMessage<bool>, string>
{
    public PauseAPI(EnvorinmentManager env): base(env) {}
    public override string Handle(SingleFieldMessage<bool> msg) {
        Debug.Log($"pause : {(bool)msg}");
        env.Pause(msg);
        return null;
    }
}


// Check and Shutdown