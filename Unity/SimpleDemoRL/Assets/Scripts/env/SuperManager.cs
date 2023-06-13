using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SuperManager : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    /////////////////////////////////////////
    private EnvorinmentManager[] environments;
    private float defaultTimeScale = 1f;
    private AgentTask task;

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

    public EnvorinmentManager GetEnv(int envNum)
    {
        return environments[envNum];
    }

    void Start()
    {
        task = new StopInTask();
        textMeshPro.SetText(task.GetDisplayName());
        environments = FindObjectsByType<EnvorinmentManager>(FindObjectsSortMode.None);
        foreach (EnvorinmentManager env in environments)
        {
            env.SetTask(task);
        }
    }
}
