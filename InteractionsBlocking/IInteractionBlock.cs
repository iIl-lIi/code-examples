using System;

public interface IInteractionBlock
{
    event Action Disposed;
    byte Depth { get; set; }

    void Block();
    bool Unblock();
}