using UnityEngine;

[System.Serializable]
public class SingleFieldMessage<T>  // This class is necessary since unity json utility can't serialize types like float, needs a wrapper
{
    public T value;

    public static implicit operator T(SingleFieldMessage<T> msg)
    {
        return msg.value;
    }
}
