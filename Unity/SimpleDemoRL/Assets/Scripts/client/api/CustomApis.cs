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

public class StepAPI: BaseEnvManagerAPI<AgentAction, EnvState>
{
    public StepAPI(EnvorinmentManager env): base(env) {}
    public override EnvState Handle(AgentAction msg) {
        //Debug.Log($"control : {msg.moveInput}, {msg.turnInput}");
        env.Step(msg);
        return env.getCurrentState();
    }
}

public class ResetAPI: BaseEnvManagerAPI<string, EnvState>
{

    public ResetAPI(EnvorinmentManager env): base(env) {}
    public override EnvState Handle(string msg) {
        //Debug.Log($"reset");
        env.Reset();
        return env.getCurrentState();
    }
}

public class PauseAPI: BaseEnvManagerAPI<SingleFieldMessage<bool>, string>
{
    public PauseAPI(EnvorinmentManager env): base(env) {}
    public override string Handle(SingleFieldMessage<bool> msg) {
        //Debug.Log($"pause : {(bool)msg}");
        env.Pause(msg);
        return null;
    }
}


public class ShutdownAPI: BaseAPI<string, string>
{
    Client client;
    public ShutdownAPI(Client c): base()
    {
        client = c;
    }
    public override string Handle(string msg) {
        Debug.Log("shutdown");
        client.ShutdownSocket("");
        return null;
    }
}
// Check