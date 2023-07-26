using Game.Item;

namespace Game.Character.Implementations.Player.States
{
    public class BasePlayerJumpOutState : AbstractPlayerState
    {
        public override string CameraAnimationName { get; set; }
        public override float AnimationCrossFadeDuration { get; set; }
        public override FPSObjectAnimationType FPSObjectAnimation { get; set; } = FPSObjectAnimationType.OnGrounded;

        public BasePlayerJumpOutState(BasePlayerController playerController) : base(playerController) { }
    }
}