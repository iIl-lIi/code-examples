using System;

public class ShootingBlock : IInteractionBlock
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
        if (Depth == byte.MinValue)
        {
            Disposed?.Invoke();
            return false;
        }
        Depth--;
        return true;
    }
}