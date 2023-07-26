using Game.Item;
using Game.PlayerStateMachine;

namespace Game.Character.Implementations.Player.States
{
    public class BasePlayerWalkState : AbstractPlayerState
    {
        public override string CameraAnimationName { get; set; } = "WalkCamera";
        public override float AnimationCrossFadeDuration { get; set; } = 0.1f;
        public override FPSObjectAnimationType FPSObjectAnimation { get; set; } = FPSObjectAnimationType.Walk;
        
        private bool _allowControlFactor;

        public BasePlayerWalkState(BasePlayerController playerController) : base(playerController) { }
        
        public override void Enter(IState fromState)
        {
            if (BasePlayerController.IsCrouch)
            {
                playerController.SwitchState<BasePlayerCrouchMoveState>();
                return;
            }
            
            cameraAnimator.StartAnimation();
            _allowControlFactor = true;
        }
        public override void Exit(IState toState)
        {
            _allowControlFactor = false;
            cameraAnimator.StopAnimation();
        }
        public override void Update()
        {
            if (!BasePlayerController.IsMove)
            {
                playerController.SwitchState<BasePlayerIdleState>();
                return;
            }
            if (BasePlayerController.IsSprint)
            {
                playerController.SwitchState<BasePlayerRunState>();
                return;
            }
            if (BasePlayerController.IsCrouch)
            {
                playerController.SwitchState<BasePlayerCrouchMoveState>();
                return;
            }

            var value = playerController.PlayerMovementInput.StickValue;
            if (_allowControlFactor) cameraAnimator.SetFactorImmediate(value);
            cameraAnimator.SetSpeedFactorImmediate(value);
        }
    }
}