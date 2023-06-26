using System;
using System.Collections.Generic;

public static class InteractionsBlocking
{
    private static readonly Dictionary<Type, IInteractionBlock> blocks = new();

    public static void Block<T>() where T : IInteractionBlock, new()
    {
        var key = typeof(T);
        if (!blocks.ContainsKey(key)) blocks.Add(key, new T());
        else blocks[key].Block();
    }
    public static void Unblock<T>() where T : IInteractionBlock
    {
        var key = typeof(T);
        if (!blocks.ContainsKey(key)) return;
        if (!blocks[key].Unblock()) blocks.Remove(key);
    }
    public static bool Get<T>(out IInteractionBlock block) where T : IInteractionBlock
    {
        var key = typeof(T);
        if (!blocks.ContainsKey(key))
        {
            block = null;
            return false;
        }
        block = blocks[key];
        return true;
    }
    public static bool IsBlocked<T>() where T : IInteractionBlock 
        => blocks.ContainsKey(typeof(T));
}