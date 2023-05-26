using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using UnityEditor;

public class Client : MonoBehaviour
{
    public string host;
    public int port;
    public EnvorinmentManager env;

    private TcpClient client;
    private NetworkStream stream;
    private Task task;
    private bool stop = false;
    private CustomApiManager apiManager;


    public void Connect()
    {
        client = new TcpClient(host, port);
        stream = client.GetStream();
        task = new Task(ReceiveMessage);
        task.Start();
    }

    private void ReceiveMessage()
    {
        Debug.Log("beginning listening for incoming msgs");
        byte[] buffer = new byte[4096];
        while (!stop)
        {
            int bytesRead;
            StringBuilder stringBuilder = new StringBuilder();

            //Debug.Log("read loop in");
            do
            {
            //Debug.Log("read loop inside");
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                stringBuilder.Append(System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead));
            } while (stream.DataAvailable);
            //Debug.Log("read loop out");

            if (stringBuilder.Length > 0)
            {
                string json = stringBuilder.ToString();
                RequestMessage message = RequestMessage.FromJson(json);
                UnityMainThreadDispatcher.Instance().Enqueue(()=>StartCoroutine(CallAPI(message))); // run api in a coroutine in main thread
            }
        }
    }

    private IEnumerator CallAPI(RequestMessage message)
    {
        ApiTuple api = apiManager.GetApi(message.api);
        yield return api.runner(message.parameter);
        string ret = api.result();
        if (ret != null) {
            ResponseMessage responseMessage = new ResponseMessage();
            responseMessage.value = ret;
            Send(responseMessage);
        }
    }

    public void ShutdownSocket(string parameter)
    {
        client.Close();

        #if UNITY_EDITOR  // FIXME : quitting app shouldn't be client responsability ?
            EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
    }

    public void Send(ResponseMessage response)
    {
        //NetworkStream stream = client.GetStream();
        byte[] data = Encoding.UTF8.GetBytes(response.ToJson());
        stream.Write(data, 0, data.Length);
    }


    // Start is called before the first frame update
    void Start()
    {
        Debug.unityLogger.logEnabled = true; // FIXME : rmove this quickfix
        //Action<string> checkAction = Check;
        //apiManager.Register("check", checkAction);
        apiManager = new CustomApiManager(env);
        apiManager.RegisterAllApis();
        Connect();
        (new ShutdownAPI(this)).Register("shutdown", apiManager);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
