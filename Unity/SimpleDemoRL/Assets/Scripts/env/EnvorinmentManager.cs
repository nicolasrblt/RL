using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EnvorinmentManager : MonoBehaviour
{
    public Client client;
    public Controller controller;
    public SpaceManager spaceManager;
    public GoalManager goalManager;
    public DummyAI dummyAI;
    public Recorder recorder;
    public RewardServerAPI serverAPI;
    public TextMeshProUGUI textMeshPro;
    public TextMeshProUGUI textMeshProRew;
    public bool manualControl = false;
    public bool rewardServerEnabled = true;
    /////////////////////////////////////////
    private float defaultTimeScale = 1f;
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
        prevAction = action;
        /*recorder.AddStep(controller.moveInput, controller.turnInput,
            spaceManager.agent.transform.position, spaceManager.agent.transform.rotation.eulerAngles,
            controller.carRigidbody.velocity, controller.carRigidbody.angularVelocity,
            spaceManager.redBall.transform.position, spaceManager.blueBall.transform.position,
            spaceManager.greenBall.transform.position, spaceManager.grayArea.transform.position,
            spaceManager.orangeArea.transform.position, spaceManager.whiteArea.transform.position);
        */
        //createCurrentState();
        //textMeshProRew.SetText($"action : {action.moveInput:0.###} | {action.turnInput:0.###}");
    }

    public void Reset()
    {
        spaceManager.Reset();
        textMeshPro.SetText(task.GetDisplayName());
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

        state.agentPostion = spaceManager.agent.transform.position;
        state.agentRotation = spaceManager.agent.transform.rotation.eulerAngles;

        state.velocity = controller.carRigidbody.velocity;
        state.angularVelocity = controller.carRigidbody.angularVelocity;

        state.redBallPosition = spaceManager.redBall.transform.position;
        state.blueBallPosition = spaceManager.blueBall.transform.position;
        state.greenBallPosition = spaceManager.greenBall.transform.position;

        state.grayAreaPosition = spaceManager.grayArea.transform.position;
        state.orangeAreaPosition = spaceManager.orangeArea.transform.position;
        state.whiteAreaPosition = spaceManager.whiteArea.transform.position;

        state.agentOutsidePlane = spaceManager.OutSidePlane(spaceManager.agent);
        state.redBallOutsidePlane = spaceManager.OutSidePlane(spaceManager.redBall);
        state.agentRedBallAngle = Vector3.Angle(spaceManager.agent.transform.forward,
                                                spaceManager.redBall.transform.position-spaceManager.agent.transform.position);
        
        if (!reset)
        {
            state.terminate = task.isFail(state) || task.isSuccess(state);
            state.reward = task.getReward(prevState, prevAction, state);
        }
        currState = state;
    }

    public void Pause(bool pause) {
        Time.timeScale = pause ? 0f : defaultTimeScale;
    }

    public void SetTimeScale(float ts) {
        defaultTimeScale = ts;
        if (Time.timeScale != 0) {
            Time.timeScale = ts;
        }
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
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
    
        recordFreq += Time.fixedDeltaTime;
        rewardCalculateFreq += Time.fixedDeltaTime;

        if (recordFreq >= 0.02f) // 1 / 50 = 0.02
        {
            recordFreq -= 0.02f;
            recorder.AddStep(controller.moveInput, controller.turnInput,
                spaceManager.agent.transform.position, spaceManager.agent.transform.rotation.eulerAngles,
                controller.carRigidbody.velocity, controller.carRigidbody.angularVelocity,
                spaceManager.redBall.transform.position, spaceManager.blueBall.transform.position,
                spaceManager.greenBall.transform.position, spaceManager.grayArea.transform.position,
                spaceManager.orangeArea.transform.position, spaceManager.whiteArea.transform.position);
        }

        if (rewardCalculateFreq >= 0.08f) // 1 / 50 = 0.02
        {
            rewardCalculateFreq -= 0.08f;
            if (recorder.GetReplaySize() > 0 & rewardServerEnabled)
            {
                var steps = recorder.SampleBuffer();
                serverAPI.SendRequest(goalManager.instruction, steps);
            }
        }
    }

    void Start()
    {
        nextAction = new AgentAction();
        task = new PushInTask();
        createCurrentState(reset: true);
        Reset();
        
    }
}
