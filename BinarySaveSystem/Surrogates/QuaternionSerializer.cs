using System.Runtime.Serialization;
using UnityEngine;

public class QuaternionSerializer : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        var vector = (Quaternion)obj;
        info.AddValue("x", vector.x);
        info.AddValue("y", vector.y);
        info.AddValue("z", vector.z);
        info.AddValue("w", vector.w);
    }
    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        var vector = (Quaternion)obj;
        vector.x = (float)info.GetValue("x", typeof(float));
        vector.y = (float)info.GetValue("y", typeof(float));
        vector.z = (float)info.GetValue("z", typeof(float));
        vector.w = (float)info.GetValue("w", typeof(float));
        return vector;
    }
}