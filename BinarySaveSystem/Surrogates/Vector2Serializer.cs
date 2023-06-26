using System.Runtime.Serialization;
using UnityEngine;

public class Vector2Serializer : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        var vector = (Vector2)obj;
        info.AddValue("x", vector.x);
        info.AddValue("y", vector.y);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        var vector = (Vector2)obj;
        vector.x = (float)info.GetValue("x", typeof(float));
        vector.y = (float)info.GetValue("y", typeof(float));
        return vector;
    }
}