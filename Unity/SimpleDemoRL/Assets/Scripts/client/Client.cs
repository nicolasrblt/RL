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
    /**
    A GameObject holding the Client communicating with python side
    
    */
    public string host;
    public int port;
    public SuperManager env;

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
        /*
        A function that blocks until it receives data from the socket, and returns the full unframed data once received
        */
        int lenBufferLen = 4;
        Debug.Log("beginning listening for incoming msgs");
        byte[] dataBuffer = new byte[4096];
        byte[] lenBuffer = new byte[10];
        int bytesRead;
        int msgLen;
        StringBuilder stringBuilder = new StringBuilder();

        while (!stop)
        {
            stringBuilder.Clear();

            // read message lenght :
            bytesRead = 0;
            while (bytesRead != lenBufferLen) {
                bytesRead += stream.Read(lenBuffer, bytesRead, lenBufferLen-bytesRead);
            }
            // If the system architecture is not little-endian, reverse the byte array.
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(lenBuffer);
            msgLen = (int)BitConverter.ToUInt32(lenBuffer, 0); // TODO : handle messages too big : overflow on sign bit / too laggy to handle
            //Debug.Log($"incomming msg size  : {msgLen} ({BitConverter.ToString(lenBuffer)})");

            
            
            
            bytesRead = 0;
            //Debug.Log("read loop in");
            while (msgLen > 0)
            {
                //Debug.Log($"read loop inside ({stream.DataAvailable})");
                bytesRead = stream.Read(dataBuffer, 0, Mathf.Min(dataBuffer.Length, msgLen));
                //Debug.Log($"did read {bytesRead}B");
                stringBuilder.Append(System.Text.Encoding.UTF8.GetString(dataBuffer, 0, bytesRead));
                msgLen -= bytesRead;
            }
            //Debug.Log("read loop out");



            if (stringBuilder.Length > 0)
            {
                try {
                string json = stringBuilder.ToString();
                //Debug.Log($"json : {json}");
                RequestMessage message = RequestMessage.FromJson(json);
                //Debug.Log($"message : {message.api}, {message.parameter}");
                // TODO : rethink the use of corroutines here : UnityMainThreadDispatcher already runs actions as coroutines
                ApiFacade api = apiManager.GetApi(message.api);
                switch (api.dispatchMethod)
                {
                    case DispatchMethod.Update:
                        UnityMainThreadDispatcher.Instance().Enqueue(()=>StartCoroutine(CallAPI(api, message))); // run api in a coroutine in main thread
                        break;
                    case DispatchMethod.FixedUpdate:
                        UnityMainThreadDispatcher.Instance().FixedEnqueue(()=>StartCoroutine(CallAPI(api, message))); // run api in a coroutine in main thread
                        break;
                    default:
                        throw new NotImplementedException();
                }
                } catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }
    }

    private IEnumerator CallAPI(ApiFacade api, RequestMessage message)
    {
        /*
        A coroutine excecuting an api
        */
        yield return StartCoroutine(api.runner(message.parameter));
        string ret = api.result();
        if (ret != null) {
            ResponseMessage responseMessage = new ResponseMessage();
            responseMessage.value = ret;
            Send(responseMessage);
        }
    }

    public void ShutdownSocket(string parameter)
    {
        /*
        Closes the sockect
        */
        stop = true;
        client.Close();
    }

    public void Send(ResponseMessage response)
    {
        /*
        Sends framed data after encoding it
        */
        byte[] data = Encoding.UTF8.GetBytes(response.ToJson());
        byte[] len = BitConverter.GetBytes(data.Length);
        // If the system architecture is not little-endian, reverse the byte array.
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(len);
        byte[] payload = Concat(len, data);
        stream.Write(payload, 0, payload.Length);
    }

    void Start()
    {
        Debug.unityLogger.logEnabled = true; // FIXME : rmove this quickfix
        //Action<string> checkAction = Check;
        //apiManager.Register("check", checkAction);
        apiManager = new CustomApiManager(env);
        apiManager.RegisterAllApis();
        Connect();
        (new ShutdownAPI(this, env)).Register("shutdown", apiManager);

    }

    public static byte[] Concat(byte[] first, byte[] second)
    {
        /*
        Concatenates 2 bytes sequences
        */
        byte[] ret = new byte[first.Length + second.Length];
        Buffer.BlockCopy(first, 0, ret, 0, first.Length);
        Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
        return ret;
    }
}
