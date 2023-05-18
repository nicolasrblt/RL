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

        agent.transform.position = position;
        agent.transform.rotation = quaternion;

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
                gameObject.transform.position = position;
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
                gameObject.transform.position = position;
                break;
            }
        }
    }

    public void Reset()
    {
        PlaceAgent();
        PlaceSphereObject(redBall);
        //PlaceSphereObject(blueBall);
        //PlaceSphereObject(greenBall);
        PlaceArea(grayArea);
        //PlaceArea(orangeArea);
        //PlaceArea(whiteArea);
    }
}
