using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class BinarySaveSystem
{
    private static string _directoryPath;
    private static BinaryFormatter _formater;

    public static void Initialize()
    {
        _directoryPath = $"{Application.persistentDataPath}/saves/"; 
        if(!Directory.Exists(_directoryPath)) Directory.CreateDirectory(_directoryPath);
        _formater = GetFormatter();
    }

    public static void Save(object data, string path)
    {
        var savePath = $"{_directoryPath}{path}";
        using(var stream = File.Create(savePath)) 
            _formater.Serialize(stream, data);
    }
    public static T Load<T>(string path) where T : new()
    {
        var loadPath = $"{_directoryPath}{path}";
        if(!File.Exists(loadPath)) return new();
        using(var stream = File.Open(loadPath, FileMode.Open))
            return (T)_formater.Deserialize(stream);
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