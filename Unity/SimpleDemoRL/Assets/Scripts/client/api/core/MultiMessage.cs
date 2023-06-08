using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
/**
* A generic Message type to send multiple values/messages in a single communication
*/
public class MultiMessage<T>
{
    public List<T> messages;
    public MultiMessage()
    {
        messages = new List<T>();
    }
}