using System;
using UnityEngine;

public class OtherInteractionBlock : IInteractionBlock
{
    public event Action Disposed;
    public byte Depth { get; set; }
    
    public OtherInteractionBlock()
    {
        Debug.Log("Other Interaction Block");
    }
    public void Block()
    {
        if (Depth == byte.MaxValue) return;
        Depth++;
    }
    public bool Unblock()
    {
        if (Depth == byte.MinValue) return false;
        Depth--;
        return true;
    }
    public void Dispose() => Disposed?.Invoke();
}