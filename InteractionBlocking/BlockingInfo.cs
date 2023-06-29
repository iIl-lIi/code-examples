using System;

public class BlockingInfo : IDisposable
{
    public bool IsBlocked { get; private set; }
    public IInteractionBlock Block { get; private set; }
    private readonly string index;

    public BlockingInfo(string index, IInteractionBlock block = null)
    {
        this.index = index;
        InteractionBlocking.Bloked += OnBlocked;
        if(block == null) return;
        block.Disposed += OnBlockDisposed;
        IsBlocked = true;
        Block = block;
    }
    public void Dispose()
    {
        InteractionBlocking.Bloked -= OnBlocked;
    }

    private void OnBlocked(string index, IInteractionBlock block)
    {
        if(IsBlocked) return;
        IsBlocked = index == this.index;
        if(IsBlocked)
        {
            Block = block;
            block.Disposed += OnBlockDisposed;
        }
    }
    private void OnBlockDisposed()
    {
        Block = null;
        IsBlocked = false;
    }
}