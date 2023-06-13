using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EnvorinmentManager : MonoBehaviour
{
    public Controller controller;
    public SpaceManager spaceManager;
    public Recorder recorder;
    public RewardServerAPI serverAPI;
    public TextMeshProUGUI textMeshProRew;
    public bool manualControl = false;
    public bool rewardServerEnabled = false;
    public bool displayLock = false;
    /////////////////////////////////////////
    private AgentTask task;
    private EnvState currState;
    private EnvState prevState;
    private AgentAction prevAction;
    private AgentAction nextAction;

    private float recordFreq = 0f;
    private float rewardCalculateFreq = 0f;

    public void Step(AgentAction action)
    {
        controller.moveInput = action.moveInput;
        controller.turnInput = action.turnInput;
        controller.newInput = true;
        prevAction = action;
        /*recorder.AddStep(controller.moveInput, controller.turnInput,
            spaceManager.agent.transform.localPosition, spaceManager.agent.transform.localRotation.eulerAngles,
            controller.carRigidbody.velocity, controller.carRigidbody.angularVelocity,
            spaceManager.redBall.transform.localPosition, spaceManager.blueBall.transform.localPosition,
            spaceManager.greenBall.transform.localPosition, spaceManager.grayArea.transform.localPosition,
            spaceManager.orangeArea.transform.localPosition, spaceManager.whiteArea.transform.localPosition);
        */
        //createCurrentState();
        //textMeshProRew.SetText($"action : {action.moveInput:0.###} | {action.turnInput:0.###}");
    }

    public void Reset()
    {
        spaceManager.Reset();
        recorder.ResetReplay(task.GetDisplayName());
        prevAction = new AgentAction();
        createCurrentState(reset: true);
    }

    public EnvState getCurrentState()
    {
        return currState;
    }

    public void createCurrentState(bool reset=false)
    {
        prevState = currState;
        EnvState state = new EnvState();

        state.agentPostion = spaceManager.agent.transform.localPosition;
        state.agentRotation = spaceManager.agent.transform.localRotation.eulerAngles;

        state.velocity = controller.carRigidbody.velocity;
        state.angularVelocity = controller.carRigidbody.angularVelocity;

        state.redBallPosition = spaceManager.redBall.transform.localPosition;
        state.blueBallPosition = spaceManager.blueBall.transform.localPosition;
        state.greenBallPosition = spaceManager.greenBall.transform.localPosition;

        state.grayAreaPosition = spaceManager.grayArea.transform.localPosition;
        state.orangeAreaPosition = spaceManager.orangeArea.transform.localPosition;
        state.whiteAreaPosition = spaceManager.whiteArea.transform.localPosition;

        state.agentOutsidePlane = spaceManager.OutSidePlane(spaceManager.agent);
        state.redBallOutsidePlane = spaceManager.OutSidePlane(spaceManager.redBall);
        state.agentRedBallAngle = spaceManager.AngleToAgent(spaceManager.redBall);
        
        state.agentGrayAreaAngle = spaceManager.AngleToAgent(spaceManager.grayArea);
        state.agentRedBallDist = spaceManager.DistToAgent(spaceManager.redBall);
        state.agentGrayAreaDist = spaceManager.DistToAgent(spaceManager.grayArea);
        state.RedBallGreyAreaDist = spaceManager.Dist(spaceManager.redBall, spaceManager.grayArea);

        if (!reset)
        {
            state.terminate = task.isFail(state) || task.isSuccess(state);
            state.reward = task.getReward(prevState, prevAction, state);
        }
        currState = state;
    }

    public void SetTask(AgentTask task)
    {
        this.task = task;
    }

    void Update()
    {
        if (manualControl) {
            nextAction.moveInput = Input.GetAxis("Vertical");
            nextAction.turnInput = Input.GetAxis("Horizontal");            
        }
    }

    void FixedUpdate()
    {
        if (manualControl) {
            Step(nextAction);
            createCurrentState();
            if (currState.terminate) {
                Reset();
            }
        }
        if (displayLock)
        {
            textMeshProRew.SetText($"angle :  {spaceManager.agent.transform.localRotation.eulerAngles.y}");
        }

        if (!rewardServerEnabled)
            return;
    
        recordFreq += Time.fixedDeltaTime;
        rewardCalculateFreq += Time.fixedDeltaTime;

        if (recordFreq >= 0.02f) // 1 / 50 = 0.02
        {
            recordFreq -= 0.02f;
            recorder.AddStep(controller.moveInput, controller.turnInput,
                spaceManager.agent.transform.localPosition, spaceManager.agent.transform.localRotation.eulerAngles,
                controller.carRigidbody.velocity, controller.carRigidbody.angularVelocity,
                spaceManager.redBall.transform.localPosition, spaceManager.blueBall.transform.localPosition,
                spaceManager.greenBall.transform.localPosition, spaceManager.grayArea.transform.localPosition,
                spaceManager.orangeArea.transform.localPosition, spaceManager.whiteArea.transform.localPosition);
        }

        if (rewardCalculateFreq >= 0.08f) // 1 / 50 = 0.02
        {
            rewardCalculateFreq -= 0.08f;
            if (recorder.GetReplaySize() > 0)
            {
                var steps = recorder.SampleBuffer();
                serverAPI.SendRequest(task.GetDisplayName(), steps);
            }
        }
    }

    void Start()
    {
        nextAction = new AgentAction();
        task = new StopInTask();
        createCurrentState(reset: true);
        Reset();
        
    }
}
