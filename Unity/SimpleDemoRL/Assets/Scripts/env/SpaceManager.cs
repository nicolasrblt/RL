using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class SpaceManager : MonoBehaviour
{
    public GameObject agent;
    public GameObject redBall;
    public GameObject blueBall;
    public GameObject greenBall;

    public GameObject grayArea;
    public GameObject orangeArea;
    public GameObject whiteArea;

    public Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> areas = new Dictionary<string, GameObject>();

    public int layerMask = 3;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    Quaternion RandomRotation()
    {
        return Quaternion.Euler(0f, UnityEngine.Random.Range(-180f, 180f), 0f);
    }

    Vector3 RandomPosition(float size)
    {
        Vector3 range = (transform.localScale - new Vector3(size, size, size) - Vector3.one) / 2;

        return new Vector3(UnityEngine.Random.Range(-range[0], range[0]), -9.5f, UnityEngine.Random.Range(-range[2], range[2]));
    }

    public void PlaceAgent()
    {
        Vector3 position = RandomPosition(1.5f);
        position[1] = 0.3f;

        Quaternion quaternion = RandomRotation();

        agent.transform.localPosition = position;
        agent.transform.localRotation = quaternion;

        Rigidbody rigidbody = agent.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }

    public void PlaceSphereObject(GameObject gameObject)
    {
        while (true)
        {
            Vector3 position = RandomPosition(0.5f);
            position[1] = 0.25f;

            if (!Physics.CheckSphere(position, 0.25f, layerMask))
            {
                gameObject.transform.localPosition = position;
                Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
                break;
            }
        }
    }

    public void PlaceArea(GameObject gameObject)
    {
        while (true)
        {
            Vector3 position = RandomPosition(1f);
            position[1] = 0.01f;
            
            if (!Physics.CheckBox(position, new Vector3(1f, 0.01f, 1f)/2, Quaternion.identity, layerMask))
            {
                gameObject.transform.localPosition = position;
                break;
            }
        }
    }

    public bool OutSidePlane(GameObject gameObject, int offset=0)
    {
        if (Mathf.Abs(gameObject.transform.localPosition.x) > 10 || Mathf.Abs(gameObject.transform.localPosition.z) > 10)
        {
            return true;
        }

        return false;
    }

    public bool OutSidePlane(Vector3 position, int offset=0)
    {
        return (Mathf.Abs(position.x) > 10-offset || Mathf.Abs(position.z) > 10-offset);
    }

    public void Reset()
    {
        float dist0 = 5.0f;
        Vector3 agentPos;
        Vector3 areaPos;
        do {
            PlaceSphereObject(redBall);

            agentPos = redBall.transform.localPosition + Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0) * Vector3.forward * dist0;
            areaPos  = redBall.transform.localPosition + Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0) * Vector3.forward * dist0;
            agentPos.y = .3f;
            areaPos.y = 0.01f;
        } while (OutSidePlane(agentPos, 2) | OutSidePlane(areaPos, 2) | (agentPos-areaPos).sqrMagnitude < 16);
        agent.transform.localPosition = agentPos;
        grayArea.transform.localPosition = areaPos;
        agent.transform.localRotation = RandomRotation();

        Rigidbody rigidbody = agent.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }

    public float DistToAgent(GameObject obj)
    {
        return Dist(agent, obj);
    }
    public float AngleToAgent(GameObject obj)
    {
        return Vector3.Angle(agent.transform.forward, obj.transform.localPosition - agent.transform.localPosition);
    }
    public float Dist(GameObject obj1, GameObject obj2)
    {
        return Vector3.Distance(obj1.transform.localPosition, obj2.transform.localPosition);
    }
}
