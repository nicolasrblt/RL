using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimeScaleAPI: Api<SingleFieldMessage<float>, string>
{
    private SuperManager env;
    public TimeScaleAPI(SuperManager env)
    {
        this.env = env;
    }
    public override string Handle(SingleFieldMessage<float> msg) {
        Debug.Log($"time scale : {(float)msg}");
        env.SetTimeScale(msg);
        return null;
    }
}

public class StepAPI: CoroutineApi<AgentAction, EnvState>
{
    private SuperManager env;
    public StepAPI(SuperManager env)
    {
        this.env = env;
    }
    public override IEnumerator Handle(AgentAction msg) {
        //Debug.Log($"control : {msg.moveInput}, {msg.turnInput}");
        env.GetEnv(0).Step(msg);
        yield return new WaitForFixedUpdate();
        env.GetEnv(0).createCurrentState();
        returnValue = env.GetEnv(0).getCurrentState();
    }
    public override DispatchMethod DispatchAt()
    {
        return DispatchMethod.FixedUpdate;
    }
}

public class ResetAPI: Api<SingleFieldMessage<int>, EnvState>
{
    private SuperManager env;
    public ResetAPI(SuperManager env)
    {
        this.env = env;
    }
    public override EnvState Handle(SingleFieldMessage<int> msg) {
        //Debug.Log($"reset");
        env.GetEnv((int)msg).Reset();
        env.GetEnv((int)msg).createCurrentState(true);
        return env.GetEnv((int)msg).getCurrentState();
    }

    public override DispatchMethod DispatchAt()
    {
        return DispatchMethod.FixedUpdate;
    }
}

public class MultiStepAPI: CoroutineApi<MultiMessage<AgentAction>, MultiMessage<EnvState>>
{
    private SuperManager env;
    public MultiStepAPI(SuperManager env)
    {
        this.env = env;
    }
    public override IEnumerator Handle(MultiMessage<AgentAction> msg) {
        returnValue = new MultiMessage<EnvState>();
        env.nStep += msg.messages.Count;
        foreach (AgentAction action in msg.messages)
        {
            env.GetEnv(action.envNum).Step(action);
        }

        yield return new WaitForFixedUpdate();

        foreach (AgentAction action in msg.messages)
        {
            env.GetEnv(action.envNum).createCurrentState();
            returnValue.messages.Add(env.GetEnv(action.envNum).getCurrentState());
        }  
    }
        public override DispatchMethod DispatchAt()
    {
        return DispatchMethod.FixedUpdate;
    }
}

public class MultiResetAPI: Api<MultiMessage<int>, MultiMessage<EnvState>>
{
    private SuperManager env;
    public MultiResetAPI(SuperManager env)
    {
        this.env = env;
    }
    public override MultiMessage<EnvState> Handle(MultiMessage<int> msg) {
        MultiMessage<EnvState> ret = new MultiMessage<EnvState>();
        foreach (int envNum in msg.messages)
        {
            env.GetEnv(envNum).Reset();
            env.GetEnv(envNum).createCurrentState(true);
            ret.messages.Add(env.GetEnv(envNum).getCurrentState());
        }        
        return ret;
    }
    public override DispatchMethod DispatchAt()
    {
        return DispatchMethod.FixedUpdate;
    }
}


public class PauseAPI: Api<SingleFieldMessage<bool>, string>
{
    private SuperManager env;
    public PauseAPI(SuperManager env)
    {
        this.env = env;
    }
    public override string Handle(SingleFieldMessage<bool> msg) {
        //Debug.Log($"pause : {(bool)msg}");
        env.Pause(msg);
        return null;
    }
}


public class ShutdownAPI: Api<string, string>
{
    private SuperManager env;
    Client client;
    public ShutdownAPI(Client c, SuperManager e): base()
    {
        client = c;
        env = e;
    }
    public override string Handle(string msg) {
        Debug.Log("shutdown");
        client.ShutdownSocket("");
        env.QuitGame();
        return null;
    }
}

public class ElapsedFUAPI: Api<string, MultiMessage<int>>
{
    private SuperManager env;
    public ElapsedFUAPI(SuperManager env)
    {
        this.env = env;
    }
    public override MultiMessage<int> Handle(string msg) {
        MultiMessage<int> returnValue = new MultiMessage<int>();
        for (int i = 0; i < 10; i++)
        {
            returnValue.messages.Add(env.GetEnv(0).controller.elapsedList[i]);
            env.GetEnv(0).controller.elapsedList[i] = 0;            
        }
        return returnValue;
    }
}

public class SpawnEnvsAPI: Api<SingleFieldMessage<int>, string>
{
    private SuperManager env;
    public SpawnEnvsAPI(SuperManager env)
    {
        this.env = env;
    }
    public override string Handle(SingleFieldMessage<int> msg) {
        env.SpawnEnvs((int)msg);
        return null;
    }
}
