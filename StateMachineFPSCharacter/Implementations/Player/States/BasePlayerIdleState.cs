using Game.Item;

namespace Game.Character.Implementations.Player.States
{
    public class BasePlayerIdleState : AbstractPlayerState
    {
        public override string CameraAnimationName { get; set; } = "IdleCamera";
        public override float AnimationCrossFadeDuration { get; set; } = 0.1f;
        public override FPSObjectAnimationType FPSObjectAnimation { get; set; } = FPSObjectAnimationType.Idle;

        public BasePlayerIdleState(BasePlayerController playerController) : base(playerController) { }
    }
}