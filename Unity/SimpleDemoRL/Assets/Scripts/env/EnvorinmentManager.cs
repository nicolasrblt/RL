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
    public TextMeshProUGUI textMeshPro;
    public TextMeshProUGUI textMeshProRew;
    public bool manualControl = false;
    /////////////////////////////////////////
    private float defaultTimeScale = 1f;
    private AgentTask task;
    private EnvState currState;
    private EnvState prevState;
    private AgentAction prevAction;
    private AgentAction nextAction;


    public void Step(AgentAction action)
    {
        controller.moveInput = action.moveInput;
        controller.turnInput = action.turnInput;
        prevAction = action;
        //createCurrentState();
        textMeshProRew.SetText($"action : {action.moveInput:0.###} | {action.turnInput:0.###}");
    }

    public void Reset()
    {
        spaceManager.Reset();
        goalManager.GenerateTask();
        //textMeshPro.SetText(goalManager.instruction);
        //recorder.ResetReplay(goalManager.instruction);
        textMeshPro.SetText(task.getDisplayName());
        recorder.ResetReplay(task.getDisplayName());
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
            if (currState.terminate) {
                Reset();
            }
        }
    }

    void Start()
    {
        nextAction = new AgentAction();
        task = new PushInTask();
        createCurrentState(reset: true);
        if (manualControl) {
            Reset();
        }
    }
}
