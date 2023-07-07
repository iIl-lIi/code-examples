using Game.Item;
using Game.PlayerStateMachine;
using OtherTools._3D.TransformAnimatorSystem;

namespace Game.Character.Implementations.Player.States
{
    public class BasePlayerCrouchIdleState : IState
    {
        private const string CAMERA_SHAKER_NAME = "CrouchIdleCamera";
        private const float CAMERA_SHAKER_CROSS_FADE_DURATION = 0.1f;
        
        public FPSObjectAnimationType FPSObjectAnimation { get; set; } = FPSObjectAnimationType.CrouchIdle;

        private readonly BasePlayerController _playerController;
        private readonly TransformAnimatorsController _animatorController;

        public BasePlayerCrouchIdleState(TransformAnimatorsController animatorController, BasePlayerController playerController)
        {
            this._playerController = playerController;
            this._animatorController = animatorController;
            
            var target = animatorController.GetAnimator(CAMERA_SHAKER_NAME).Target;
            playerController.TargetPlayerCharacter.AddOffsets.Add(new AddOffset()
            {
                UseParameters = UseTransformParameters.Everything,
                GetPosition = () => target.localPosition,
                GetRotation = () => target.localRotation
            });
        }

        public void Enter(IState fromState)
        {
            _playerController.TargetPlayerCharacter.Crouch(true);
            _animatorController.Play(CAMERA_SHAKER_NAME, CAMERA_SHAKER_CROSS_FADE_DURATION);
        }
        public void Exit(IState toState)
        {
            _animatorController.Stop(CAMERA_SHAKER_NAME, CAMERA_SHAKER_CROSS_FADE_DURATION);
            if (toState is BasePlayerCrouchMoveState) return;
            _playerController.TargetPlayerCharacter.Crouch(false);
        }
        public void Update()
        {
            if (BasePlayerController.CrouchMovement)
            {
                _playerController.SwitchState<BasePlayerCrouchMoveState>();
                return;
            }
            
            if (BasePlayerController.IsCrouch) return;
            _playerController.SwitchState<BasePlayerIdleState>();
        }
    }
}