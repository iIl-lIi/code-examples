using Game.Item;
using Game.PlayerStateMachine;

namespace Game.Character.Implementations.Player.States
{
    public class BasePlayerCrouchIdleState : AbstractPlayerState
    {
        private const string CAMERA_SHAKER_NAME = "CrouchIdleCamera";
        private const float CAMERA_SHAKER_CROSS_FADE_DURATION = 0.1f;
        
        public override string CameraAnimationName { get; set; }
        public override float AnimationCrossFadeDuration { get; set; }
        public override FPSObjectAnimationType FPSObjectAnimation { get; set; } = FPSObjectAnimationType.CrouchIdle;

        public BasePlayerCrouchIdleState(BasePlayerController playerController) : base(playerController) { }

        public override void Enter(IState fromState)
        {
            playerController.TargetPlayerCharacter.Crouch(true);
            base.Enter(fromState);
        }
        public override void Exit(IState toState)
        {
            base.Exit(toState);
            if (toState is BasePlayerCrouchMoveState) return;
            playerController.TargetPlayerCharacter.Crouch(false);
        }
        public override void Update()
        {
            if (BasePlayerController.CrouchMovement)
            {
                playerController.SwitchState<BasePlayerCrouchMoveState>();
                return;
            }
            
            if (BasePlayerController.IsCrouch) return;
            playerController.SwitchState<BasePlayerIdleState>();
        }
    }
}