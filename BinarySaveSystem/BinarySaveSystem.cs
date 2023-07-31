using Extensions;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class BinarySaveSystem
{
    private static readonly BinaryFormatter _formater = GetFormatter();

    public static void Save(object data, string path)
    {
        CheckDirectory(path);
        using(var stream = File.Create(path)) 
            _formater.Serialize(stream, data);
    }
    public static T Load<T>(string path) where T : new()
    {
        CheckDirectory(path);
        if(!File.Exists(path)) return new();
        using(var stream = File.Open(path, FileMode.Open))
            return (T)_formater.Deserialize(stream);
    }
    
    private static void CheckDirectory(string path)
    {
        var fileFullName = path.GetFileFullNameFromPath();
        var correctPath = path.RemoveRight(fileFullName);
        if (Directory.Exists(correctPath)) return;
        Directory.CreateDirectory(correctPath);
    }
    private static BinaryFormatter GetFormatter()
    {
        var b = new BinaryFormatter();
        var s = new SurrogateSelector();
        s.AddSurrogate(typeof(Vector2),    new StreamingContext(StreamingContextStates.All), new Vector2Serializer());
        s.AddSurrogate(typeof(Vector3),    new StreamingContext(StreamingContextStates.All), new Vector3Serializer());
        s.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), new QuaternionSerializer());
        b.SurrogateSelector = s;
        return b;
    }
}