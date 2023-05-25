using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    public GameObject agent;
    public GameObject redBall;
    //public GameObject blueBall;
    //public GameObject greenBall;

    public GameObject grayArea;
    //public GameObject orangeArea;
    //public GameObject whiteArea;

    public Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> areas = new Dictionary<string, GameObject>();
    public Dictionary<string, Func<GameObject, GameObject, bool>> functions = new Dictionary<string, Func<GameObject, GameObject, bool>>();

    public GameObject targetObject;
    public GameObject areaObject;
    public Func<GameObject, GameObject, bool> func;
    public string instruction;
    public TaskEnum task;

    // Start is called before the first frame update
    void Start()
    {
        objects.Add("agent", agent);
        objects.Add("red ball", redBall);
        //objects.Add("blue ball", blueBall);
        //objects.Add("green ball", greenBall);

        areas.Add("gray area", grayArea);
        //areas.Add("orange area", orangeArea);
        //areas.Add("white area", whiteArea);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateTask()
    {
        //List<string> objectKeys = new List<string>(objects.Keys);
        //List<string> areaKeys = new List<string>(areas.Keys);

        //string randomObjectKey = objectKeys[UnityEngine.Random.Range(0, objectKeys.Count)];
        //string randomAreaKey = areaKeys[UnityEngine.Random.Range(0, areaKeys.Count)];
        //targetObject = objects[randomObjectKey];
        //areaObject = areas[randomAreaKey];


        int task_index = UnityEngine.Random.Range(0, 3);

        switch (task_index)
        {
            case 0:
                targetObject = agent;
                areaObject = grayArea;
                func = StopInFunction;
                instruction = "Stop the agent in " + "gray" + " area";
                task = TaskEnum.StopIn;

                break;

            case 1:
                targetObject = redBall;
                areaObject = grayArea;
                func = PushInFunction;
                instruction = "Push the " + "red ball" + " in " + "gray" + " area";
                task = TaskEnum.PushIn;
                break;

            default:
                targetObject = redBall;
                areaObject = grayArea;
                targetObject.transform.position = areaObject.transform.position;
                func = PushOutFunction;
                instruction = "Push the " + "red ball" + " out " + "gray" + " area";
                task = TaskEnum.PushOut;
                break;
        }
    }

    public bool StopInFunction(GameObject gameObject, GameObject area)
    {
        bool terminate = false;

        float distance = Vector3.Distance(gameObject.transform.position, area.transform.position);
        Rigidbody rigidbody = agent.GetComponent<Rigidbody>();
        Vector3 speed = rigidbody.velocity;

        if(distance <= 0.7f)
        {
            terminate = true;
        }

        return terminate;
    }

    public bool PushInFunction(GameObject gameObject, GameObject area)
    {
        bool terminate = false;

        float distance = Vector3.Distance(gameObject.transform.position, area.transform.position);
        Rigidbody rigidbody = agent.GetComponent<Rigidbody>();
        Vector3 speed = rigidbody.velocity;

        if (distance <= 0.5)
        {
            terminate = true;
        }

        return terminate;
    }

    public bool PushOutFunction(GameObject gameObject, GameObject area)
    {
        bool terminate = false;

        float distance = Vector3.Distance(gameObject.transform.position, area.transform.position);
        Rigidbody rigidbody = agent.GetComponent<Rigidbody>();
        Vector3 speed = rigidbody.velocity;

        if (distance > 1f)
        {
            terminate = true;
        }

        return terminate;
    }
}
