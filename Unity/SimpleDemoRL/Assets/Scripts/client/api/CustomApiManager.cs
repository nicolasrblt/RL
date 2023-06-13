using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomApiManager : APIManager
{
    SuperManager env;

    public CustomApiManager(SuperManager env): base()
    {
        this.env = env;
    }

    public override void RegisterAllApis() {
        (new TimeScaleAPI(env)).Register("timeScale", this);
        (new PauseAPI(env)).Register("pause", this);
        (new StepAPI(env)).Register("step", this);
        (new ResetAPI(env)).Register("reset", this);
        (new MultiStepAPI(env)).Register("multiStep", this);
        (new MultiResetAPI(env)).Register("multiReset", this);
        (new ElapsedFUAPI(env)).Register("elapsedFU", this);
    }
}
