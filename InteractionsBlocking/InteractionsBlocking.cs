using System;
using System.Collections.Generic;

public static class InteractionsBlocking
{
    public static event Action<Type, IInteractionBlock> Bloked;
    private static readonly Dictionary<Type, IInteractionBlock> blocks = new ();

    public static void Block<T>() where T : IInteractionBlock, new ()
    {
        var key = typeof(T);
        if (!blocks.ContainsKey(key))
        {
            var newT = new T();
            blocks.Add(key, newT);
            Bloked?.Invoke(key, newT);
        }
        else blocks[key].Block();
    }
    public static void Unblock<T>() where T : IInteractionBlock
    {
        var key = typeof(T);
        if (!blocks.ContainsKey(key)) return;
        if (!blocks[key].Unblock()) blocks.Remove(key);
    }
    public static BlockingInfo<T> GetInfo<T>() where T : IInteractionBlock, new ()
    {
        var key = typeof(T);
        if (blocks.ContainsKey(key)) return new (blocks[key]);
        return new ();
    }
    public static bool IsBlocked<T>() where T : IInteractionBlock 
        => blocks.ContainsKey(typeof(T));
}