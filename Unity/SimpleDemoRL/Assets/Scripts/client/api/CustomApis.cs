using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class EnvAPI<ArgType, RetType>: Api<ArgType, RetType>
{
    protected EnvorinmentManager env;

    public EnvAPI(EnvorinmentManager target): base()
    {
        env = target;
    }
}

public abstract class EnvCoroutineAPI<ArgType, RetType>: CoroutineApi<ArgType, RetType>
{
    protected EnvorinmentManager env;

    public EnvCoroutineAPI(EnvorinmentManager target): base()
    {
        env = target;
    }
}

public class TimeScaleAPI: EnvAPI<SingleFieldMessage<float>, string>
{
    public TimeScaleAPI(EnvorinmentManager env): base(env) {}
    public override string Handle(SingleFieldMessage<float> msg) {
        Debug.Log($"time scale : {(float)msg}");
        env.SetTimeScale(msg);
        return null;
    }
}

public class StepAPI: EnvCoroutineAPI<AgentAction, EnvState>
{
    public StepAPI(EnvorinmentManager env): base(env) {}
    public override IEnumerator Handle(AgentAction msg) {
        //Debug.Log($"control : {msg.moveInput}, {msg.turnInput}");
        env.Step(msg);
        yield return new WaitForFixedUpdate();
        env.createCurrentState();
        returnValue = env.getCurrentState();
    }
}

public class ResetAPI: EnvCoroutineAPI<string, EnvState>
{

    public ResetAPI(EnvorinmentManager env): base(env) {}
    public override IEnumerator Handle(string msg) {
        //Debug.Log($"reset");
        env.Reset();
        yield return new WaitForFixedUpdate();
        env.createCurrentState(true);
        returnValue = env.getCurrentState();
    }
}

public class PauseAPI: EnvAPI<SingleFieldMessage<bool>, string>
{
    public PauseAPI(EnvorinmentManager env): base(env) {}
    public override string Handle(SingleFieldMessage<bool> msg) {
        //Debug.Log($"pause : {(bool)msg}");
        env.Pause(msg);
        return null;
    }
}


public class ShutdownAPI: Api<string, string>
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