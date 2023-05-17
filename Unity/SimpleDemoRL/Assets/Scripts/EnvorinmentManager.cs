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
    public bool playMod;
    private float timeSinceLastStep = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Action<string> resetAction = ResetAPI;
        Action<string> stepAction = StepAPI;

        client.apiManager.Register("reset", resetAction);
        client.apiManager.Register("step", stepAction);
    }

    public void ResetAPI(string parameter)
    {
        Reset();
        ObservationMessage observationMessage = GetObservation();

        ResponseMessage responseMessage = new ResponseMessage();
        responseMessage.value = observationMessage.ToJson();
        client.Send(responseMessage);
    }

    public void StepAPI(string parameter)
    {
        ControllMessage controllMessage = ControllMessage.FromJson(parameter);
        Step(controllMessage);
        ObservationMessage observationMessage = GetObservation();


        ResponseMessage responseMessage = new ResponseMessage();
        responseMessage.value = observationMessage.ToJson();
        client.Send(responseMessage);
    }

    public void Step(ControllMessage controllMessage)
    {
        controller.moveInput = controllMessage.moveInput;
        controller.turnInput = controllMessage.turnInput;
        //controller.Step();
    }

    private ObservationMessage GetObservation()
    {
        ObservationMessage observationMessage = new ObservationMessage();
        observationMessage.agentPostion = spaceManager.agent.transform.position;
        observationMessage.agentRotation = spaceManager.agent.transform.rotation.eulerAngles;

        observationMessage.velocity = controller.carRigidbody.velocity;
        observationMessage.angularVelocity = controller.carRigidbody.angularVelocity;

        observationMessage.redBallPosition = spaceManager.redBall.transform.position;
        observationMessage.blueBallPosition = spaceManager.blueBall.transform.position;
        observationMessage.greenBallPosition = spaceManager.greenBall.transform.position;

        observationMessage.grayAreaPosition = spaceManager.grayArea.transform.position;
        observationMessage.orangeAreaPosition = spaceManager.orangeArea.transform.position;
        observationMessage.whiteAreaPosition = spaceManager.whiteArea.transform.position;

        
        float reward;
        bool terminate;

        (terminate, reward) = EndGame();

        observationMessage.terminate = terminate;
        observationMessage.reward = reward;

        return observationMessage;
    }

    // Update is called once per frame
    void Update()
    {       
        //if (EndGame())
        //{
        //    Reset();
        //}

        //Debug.Log(rewardCalculator.GetInstructionTensor(goalManager.task));
    }

    void FixedUpdate()
    {
        
    }

    private string GenerateName()
    {
        string date = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string replayName = "Replay_" + date;

        return replayName;
    }

    private (bool, float) EndGame()
    {
        bool terminated = false;

        if (goalManager.func(goalManager.targetObject, goalManager.areaObject))
        {
            return (true, 1f);
        }

        if (OutSidePlane(spaceManager.agent))
        {
            return (true, -1f);
        }

        if (OutSidePlane(spaceManager.redBall))
        {
            return (true, -1f);
        }
        /*
        if (OutSidePlane(spaceManager.blueBall))
        {
            return true;
        }

        if (OutSidePlane(spaceManager.greenBall))
        {
            return true;
        }
        */

        if (recorder.GetReplaySize() > 1000)
        {
            return (true, -1f);
        }

        return (terminated, 0);
    }

    private bool OutSidePlane(GameObject gameObject)
    {
        if (Mathf.Abs(gameObject.transform.position.x) > 10 || Mathf.Abs(gameObject.transform.position.z) > 10)
        {
            return true;
        }

        return false;
    }

    public void Reset()
    {
        spaceManager.Reset();
        goalManager.GenerateTask();
        textMeshPro.SetText(goalManager.instruction);
        recorder.ResetReplay(goalManager.instruction);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
        #else
                Application.Quit();
        #endif
    }

}
