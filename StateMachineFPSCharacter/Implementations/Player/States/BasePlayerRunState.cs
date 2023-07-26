using Game.Item;
using Game.PlayerStateMachine;

namespace Game.Character.Implementations.Player.States
{
    public class BasePlayerRunState : AbstractPlayerState
    {   
        public override string CameraAnimationName { get; set; } = "RunCamera";
        public override float AnimationCrossFadeDuration { get; set; } = 0.2f;
        public override FPSObjectAnimationType FPSObjectAnimation { get; set; } = FPSObjectAnimationType.Run;

        public BasePlayerRunState(BasePlayerController playerController) : base(playerController) { }
        
        public override void Enter(IState fromState)
        {
            base.Enter(fromState);
            InteractionBlocking.Block(Constants.AimingBlock);
        }
        public override void Exit(IState toState)
        {
            base.Exit(toState);
            InteractionBlocking.Unblock(Constants.AimingBlock);
        }
        public override void Update()
        {
            if (BasePlayerController.IsSprint == false || BasePlayerController.IsCrouch)
                playerController.SwitchState<BasePlayerWalkState>();
        }
    }
}