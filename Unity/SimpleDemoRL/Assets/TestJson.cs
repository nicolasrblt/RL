using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class RandomMessage
{
    public float x;
    public float y;
}

[System.Serializable]
class MultiRandomMessage
{
    public List<RandomMessage> msgList;
    public MultiRandomMessage()
    {
        msgList = new List<RandomMessage>();
    }
}


public class TestJson : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RandomMessage msg1 = new RandomMessage();
        RandomMessage msg2 = new RandomMessage();
        RandomMessage msg3 = new RandomMessage();
        msg1.x = 1;
        msg1.y = 10;
        msg3.x = 3;
        msg3.y = 33;

        MultiRandomMessage msg = new MultiRandomMessage();
        msg.msgList.Add(msg1);
        msg.msgList.Add(msg2);
        msg.msgList.Add(msg3);

        string json = UnityEngine.JsonUtility.ToJson(msg);
        Debug.Log(json);
        Debug.Log(JsonUtility.ToJson(msg.msgList));
        Debug.Log(JsonUtility.FromJson<List<RandomMessage>>(JsonUtility.ToJson(msg.msgList)));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
