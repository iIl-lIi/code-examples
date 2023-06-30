using System;

public class BaseInteractionBlock : IInteractionBlock
{
    public event Action Disposed;
    public byte Depth { get; set; }

    public void Block()
    {
        if (Depth == byte.MaxValue) return;
        Depth++;
    }
    public bool Unblock()
    {
        if (Depth == byte.MinValue) return false;
        return --Depth > byte.MinValue;
    }
    public void Dispose() => Disposed?.Invoke();
}