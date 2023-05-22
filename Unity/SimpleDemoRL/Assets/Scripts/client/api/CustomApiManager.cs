using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomApiManager : APIManager
{
    EnvorinmentManager env;

    public CustomApiManager(EnvorinmentManager env): base()
    {
        this.env = env;
    }

    public override void RegisterAllApis() {
        Register("test", TestApi.GetAPI<TestApi>());
        (new TimeScaleAPI(env)).Register("timeScale", this);
        (new PauseAPI(env)).Register("pause", this);
        (new StepAPI(env)).Register("step", this);
        (new ResetAPI(env)).Register("reset", this);
    }
}
