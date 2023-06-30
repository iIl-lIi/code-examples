using System;
using System.Collections.Generic;

public static class InteractionBlocking
{
    public static event Action<string, IInteractionBlock> Blocked;
    public static event Action<string> Reblocked;
    public static event Action<string> Unblocked;
    private static readonly Dictionary<string, IInteractionBlock> blocks = new ();

    public static void Block(string key, IInteractionBlock block = null) => AddBlock(key, block);
    public static void Unblock(string key, bool forcedSave = false)
    {
        if (!blocks.ContainsKey(key)) return;
        var saveBlock = blocks[key].Unblock();
        Unblocked?.Invoke(key);
        if (saveBlock || forcedSave) return;
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
            block = blocks[key];
            block.Block();
            Reblocked?.Invoke(key);
            return;
        }
        if (block == null) block = new BaseInteractionBlock();
        blocks.Add(key, block);
        Blocked?.Invoke(key, block);
    }
}