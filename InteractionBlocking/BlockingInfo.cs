using System;

public class BlockingInfo : IDisposable
{
    public bool IsBlocked { get; private set; }
    public IInteractionBlock BlockInstance { get; private set; }

    private string index;

    public BlockingInfo(string index, IInteractionBlock blockInstance = null)
    {
        this.index = index;
        InteractionBlocking.Blocked   += OnBlocked;
        InteractionBlocking.Reblocked += OnReblocked;
        InteractionBlocking.Unblocked += OnUnblocked;
        if (blockInstance == null) return;

        IsBlocked = true;
        blockInstance.Disposed += OnBlockDisposed;
        BlockInstance = blockInstance;
    }
    public void Dispose()
    {
        if(index == string.Empty) return;
        InteractionBlocking.Blocked   -= OnBlocked;
        InteractionBlocking.Reblocked -= OnReblocked;
        InteractionBlocking.Unblocked -= OnUnblocked;
        index = string.Empty;
    }
    public void Block() => InteractionBlocking.Block(index);
    public void Unblock(bool forcedSave = false) => InteractionBlocking.Unblock(index, forcedSave);
    
    private void OnBlocked(string index, IInteractionBlock block)
    {
        if (IsBlocked || index != this.index) return;
        BlockInstance = block;
        block.Disposed += OnBlockDisposed;
        IsBlocked = true;
    }
    private void OnReblocked(string index)
    {
        if (IsBlocked || index != this.index) return;
        IsBlocked = true;
    }
    private void OnUnblocked(string index)
    {
        if (!IsBlocked || index != this.index) return;
        IsBlocked = BlockInstance.Depth > 0;
    }
    private void OnBlockDisposed()
    {
        BlockInstance = null;
        IsBlocked = false;
    }
}