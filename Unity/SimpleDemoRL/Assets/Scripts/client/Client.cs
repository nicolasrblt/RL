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
    //public Manager manager;

    private TcpClient client;
    private NetworkStream stream;
    //private Thread receiveThread;
    private Task task;
    private bool stop = false;
    public APIManager apiManager = new APIManager();

    public void Connect()
    {
        client = new TcpClient(host, port);
        stream = client.GetStream();
        task = new Task(ReceiveMessage);
        task.Start();
        //receiveThread = new Thread(ReceiveMessage);
        //receiveThread.Start();
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
                CallAPI(message);
            }
        }
    }

    private void CallAPI(RequestMessage message)
    {
        Debug.Log("calling "+message.api+" | "+message.parameter);
        string ret = apiManager.Call(message.api, message.parameter);
        Debug.Log("return : "+ret);
        ResponseMessage responseMessage = new ResponseMessage();
        responseMessage.value = ret;
        Send(responseMessage);
        Debug.Log("sent back "+responseMessage.ToJson());
    }

    public void ShutdownSocket(string parameter)
    {
        //stop = true;

        //if (!receiveThread.Join(5000)) // Wait for up to 5 seconds for the thread to stop
        //{
        //    receiveThread.Abort(); // Force the thread to stop if it didn't within the given time
        //}
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
        //Action<string> checkAction = Check;
        //apiManager.Register("check", checkAction);
        Connect();
        Action<string> shutdown = ShutdownSocket;
        //apiManager.Register("shutdown", shutdown);  // TODO fix api
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}