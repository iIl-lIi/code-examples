using System;
using System.Collections.Generic;

public static class InteractionBlocking
{
    public static event Action<string, IInteractionBlock> Bloked;
    private static readonly Dictionary<string, IInteractionBlock> blocks = new ();

    public static void Block    (string key, IInteractionBlock block = null) => AddBlock(key, block);
    public static bool IsBlocked(string key) => blocks.ContainsKey(key);
    public static void Unblock  (string key)
    {
        if (!blocks.ContainsKey(key)) return;
        if (blocks[key].Unblock())    return;
        blocks[key].Dispose();
        blocks.Remove(key);
    }
    public static BlockingInfo GetInfo(string key)
    {
        if (blocks.ContainsKey(key)) return new (key, blocks[key]);
        return new (key);
    }

    private static void AddBlock(string key, IInteractionBlock block)
    {
        if (blocks.ContainsKey(key))
        {
            blocks[key].Block();
            return;
        }
        if(block == null) block = new BaseInteractionBlock();
        blocks.Add(key, block);
        Bloked?.Invoke(key, block);
    }
}