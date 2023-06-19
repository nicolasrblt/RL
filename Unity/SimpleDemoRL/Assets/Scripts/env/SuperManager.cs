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
    public GameObject TrainingArea;
    public GameObject mainCamera;
    /////////////////////////////////////////
    private List<EnvorinmentManager> environments;
    private float defaultTimeScale = 1f;
    private AgentTask task;
    public int nStep = 0;

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
        //SpawnEnvs(25);
        environments = new List<EnvorinmentManager>();
        foreach (EnvorinmentManager env in environments)
        {
            env.SetTask(task);
        }
    }

    public void SpawnEnvs(int n)
    {
        foreach (EnvorinmentManager env in environments)
        {
            Destroy(env.gameObject.transform.parent.gameObject, 0);
            Debug.Log("destroy");
        }
        environments.Clear();

        int c = Mathf.CeilToInt(Mathf.Sqrt(n));
        for (int i = 0; i*c<n; i++)
        {
            for (int j = 0; i+j*c<n; j++)
            {
                environments.Add(
                    Instantiate(TrainingArea, new Vector3((i-c/2f+.5f)*25, 0, (j-c/2f+.5f)*25), Quaternion.identity).GetComponent<EnvorinmentManager>()
                );
                Debug.Log("instantiate");
            }
        }
        mainCamera.transform.position = new Vector3(0, c*30, 0);
    }
}
