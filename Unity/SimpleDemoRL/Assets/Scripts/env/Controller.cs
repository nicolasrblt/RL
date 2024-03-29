using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
A GameObject managing the agent actions
Allow to register an action executed each fixed update until a new one is registered
*/
public class Controller : MonoBehaviour
{
    public float speed = 10.0f; // forward movement speed
    public float turnSpeed = 50.0f; // turning speed
    public Rigidbody carRigidbody; // rigidbody of the car
    public float moveInput; // input for forward movement
    public float turnInput; // input for turning
    public List<int> elapsedList;
    public bool manualControl = false;
    private int elapsedFU = 0;
    public bool newInput = false;

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        elapsedList = new List<int> ();
        for (int i = 0; i < 10; i++)
        {
            elapsedList.Add(0);
        }
    }

    void Update()
    {
        if (manualControl) {
        // get input for forward movement and turning
        //moveInput = Input.GetAxis("Vertical");
        //turnInput = Input.GetAxis("Horizontal");            
        }

    }

    void FixedUpdate()
    {
        if (newInput)
        {
            Debug.Log($"{elapsedFU} FU elsapsed TS={Time.timeScale}");
            newInput  = false;
            if (elapsedFU < 10)
            {
                elapsedList[elapsedFU]++;
            }
            elapsedFU = 0;
        }
        elapsedFU++;

        // move the car forward or backward
        Vector3 movement = transform.forward * moveInput * speed;
        carRigidbody.AddForce(movement);

        // turn the car left or right
        Quaternion turn = Quaternion.Euler(0, turnInput * turnSpeed * Time.fixedDeltaTime, 0);
        carRigidbody.MoveRotation(carRigidbody.rotation * turn);

        // stop the car if space is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            carRigidbody.velocity = Vector3.zero;
            carRigidbody.angularVelocity = Vector3.zero;
        }
    }
}
