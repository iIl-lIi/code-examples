public class ShootingBlock : IInteractionBlock
{
    public byte BlockDepth { get; set; }

    public void Block()
    {
        if (BlockDepth == byte.MaxValue) return;
        BlockDepth++;
    }
    public bool Unblock()
    {
        if (BlockDepth == byte.MinValue) return false;
        BlockDepth--;
        return true;
    }
}