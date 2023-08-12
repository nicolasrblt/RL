using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

class ReplayRequest
{
    public string instruction;
    public List<string> replay;
}

class PredictResponse
{
    public float predictions;
}

public class RewardServerAPI : MonoBehaviour  // TODO : doesn't need to be a monobehaviour, instance should be shared by all training areas
{
    public string apiUrl = "http://localhost:8000/predict";
    public TextMeshProUGUI textMeshPro;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    List<string> PrepareReplay(List<Step> steps)
    {
        List<string> features = new List<string>();

        foreach (var step in steps)
        {
            features.Add(JsonUtility.ToJson(step));
        }

        return features;
    }

    public string PrepareJson(string instruction, List<Step> steps)
    {
        var request = new ReplayRequest();
        request.instruction = instruction;
        request.replay = PrepareReplay(steps);

        string requestJson = JsonUtility.ToJson(request);

        return requestJson;
    }

    private IEnumerator PostRequest(string url, string bodyJsonString)
    {
        using (var request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error: " + request.error);
            }
            else
            {
                var prediction = JsonUtility.FromJson<PredictResponse>(request.downloadHandler.text);
                textMeshPro.SetText("Reward: " + prediction.predictions.ToString("F4"));
            }
        }
    }

    public void SendRequest(string instruction, List<Step> steps)
    {
        var jsonString = PrepareJson(instruction, steps);
        StartCoroutine(PostRequest(apiUrl, jsonString));
    }

}
