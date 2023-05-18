using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public Client client;
    private bool isPaused = false;

    private void Start()
    {
        Action<string> resetAction = PauseAPI;

        //client.apiManager.Register("pause", resetAction);
    }

    public void PauseAPI(string parameter)
    {
        if (parameter == "pause")
        {
            Pause();
        }
        else
        {
            ResumeGame();
        }
    }

    void Update()
    {
        
    }

    void Pause()
    {
        Time.timeScale = 0f;
        isPaused = true;
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }
}
