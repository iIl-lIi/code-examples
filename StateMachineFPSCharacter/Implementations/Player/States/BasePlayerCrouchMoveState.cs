using Game.Item;
using Game.PlayerStateMachine;

namespace Game.Character.Implementations.Player.States
{
    public class BasePlayerCrouchMoveState : AbstractPlayerState
    {
        public override string CameraAnimationName { get; set; } = "CrouchMoveCamera";
        public override float AnimationCrossFadeDuration { get; set; } = 0.1f;
        public override FPSObjectAnimationType FPSObjectAnimation { get; set; } = FPSObjectAnimationType.CrouchMove;

        private bool _allowControlFactor;

        public BasePlayerCrouchMoveState(BasePlayerController playerController) : base(playerController) { }

        public override void Enter(IState fromState)
        {
            playerController.TargetPlayerCharacter.Crouch(true);
            cameraAnimator.StartAnimation();
            _allowControlFactor = true;
        }
        public override void Exit(IState toState)
        {
            _allowControlFactor = false;
            cameraAnimator.StopAnimation();
            if (toState is BasePlayerCrouchIdleState) return;
            playerController.TargetPlayerCharacter.Crouch(false);
        }
        public override void Update()
        {
            if (BasePlayerController.IsCrouch && BasePlayerController.CrouchMovement == false)
            {
                playerController.SwitchState<BasePlayerCrouchIdleState>();
                return;
            }
            if (BasePlayerController.IsCrouch == false)
            {
                if (BasePlayerController.IsSprint) playerController.SwitchState<BasePlayerRunState>();
                else playerController.SwitchState<BasePlayerWalkState>();
                return;
            }
            
            var value = playerController.PlayerMovementInput.StickValue;
            if (_allowControlFactor) cameraAnimator.SetFactorImmediate(value);
            cameraAnimator.SetSpeedFactorImmediate(value);
        }
        
        private void OnEnterSetFactor()
        {
            _allowControlFactor = true;
        }
    }
}