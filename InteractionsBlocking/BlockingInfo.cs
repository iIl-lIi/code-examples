using System;

public class BlockingInfo<T> : IDisposable where T : IInteractionBlock
{
    public bool IsBlocked { get; private set; }
    public IInteractionBlock Block  { get; private set; }

    public BlockingInfo(IInteractionBlock block = null)
    {
        InteractionsBlocking.Bloked += OnGlobalBlocked;
        if(block == null) return;
        block.Disposed += OnBlockDisposed;
        IsBlocked = true;
        Block = block;
    }
    public void Dispose()
    {
        InteractionsBlocking.Bloked -= OnGlobalBlocked;
    }

    private void OnGlobalBlocked(Type type, IInteractionBlock block)
    {
        if(IsBlocked) return;
        IsBlocked = type == typeof(T);
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