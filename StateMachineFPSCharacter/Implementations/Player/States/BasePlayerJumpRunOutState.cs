using Game.Item;

namespace Game.Character.Implementations.Player.States
{
    public class BasePlayerJumpRunOutState : AbstractPlayerState
    {
        public override FPSObjectAnimationType FPSObjectAnimation { get; set; } = FPSObjectAnimationType.OnGroundedRun;
        
        public override string CameraAnimationName { get; set; }
        public override float AnimationCrossFadeDuration { get; set; }
        
        public BasePlayerJumpRunOutState(BasePlayerController playerController) : base(playerController) { }
    }
}