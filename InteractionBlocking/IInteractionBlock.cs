using System;

public interface IInteractionBlock : IDisposable
{
    event Action Disposed;
    byte Depth { get; set; }

    void Block();
    bool Unblock();
}