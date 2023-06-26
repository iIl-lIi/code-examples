using System;
using System.Collections.Generic;
using UnityEngine;

public static class InteractionBlocking
{
    public static event Action<string, IInteractionBlock> Bloked;
    private static readonly Dictionary<string, IInteractionBlock> blocks = new ();

    public static void Block<T>(string key) where T : IInteractionBlock, new ()
    {
        if (!blocks.ContainsKey(key))
        {
            var newInteractionBlock = new T();
            blocks.Add(key, newInteractionBlock);
            Bloked?.Invoke(key, newInteractionBlock);
        }
        else blocks[key].Block();
    }
    public static void Unblock(string key)
    {
        if (!blocks.ContainsKey(key)) return;
        if (!blocks[key].Unblock())
        {
            blocks[key].Dispose();
            blocks.Remove(key);
        }
    }
    public static BlockingInfo GetInfo(string key)
    {
        if (blocks.ContainsKey(key)) return new (key, blocks[key]);
        return new (key);
    }
    public static bool IsBlocked(string key) => blocks.ContainsKey(key);
}