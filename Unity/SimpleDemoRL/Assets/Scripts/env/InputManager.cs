using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
An input manager allowing the agent to be controlled by the user
*/
public class InputManager : MonoBehaviour
{
    public Controller controller;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        controller.moveInput = Input.GetAxis("Vertical");
        controller.turnInput = Input.GetAxis("Horizontal");
    }
}
