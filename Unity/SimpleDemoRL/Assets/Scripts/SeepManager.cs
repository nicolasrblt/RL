using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeepManager : MonoBehaviour
{

    public float newTimeScale = 2.0f;

    void Start()
    {
        Time.timeScale = newTimeScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
