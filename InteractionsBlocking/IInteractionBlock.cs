public interface IInteractionBlock
{
    byte BlockDepth { get; set; }

    void Block();
    bool Unblock();
}