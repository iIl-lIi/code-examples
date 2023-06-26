using Game.FPSObject;
using Game.StateMachine.Player;
using OtherTools._3D.TransformAnimatorSystem;

namespace Game.Character.Implementations.Player.States
{
    public class BasePlayerCrouchIdleState : IBaseState
    {
        private const string CAMERA_SHAKER_NAME = "CrouchIdleCamera";
        private const float CAMERA_SHAKER_CROSS_FADE_DURATION = 0.1f;
        
        public FPSObjectShakerType FPSObjectShaker { get; set; } = FPSObjectShakerType.CrouchIdle;

        private readonly BasePlayerController _playerController;
        private readonly TransformAnimatorsController _animatorController;

        public BasePlayerCrouchIdleState(TransformAnimatorsController animatorController, BasePlayerController playerController)
        {
            this._playerController = playerController;
            this._animatorController = animatorController;
            
            playerController.TargetPlayerCharacter.AddTransforms.Add(new AddTransform()
            {
                UseParameters = UseTransformParameters.Everything,
                Transform = animatorController.GetShaker(CAMERA_SHAKER_NAME).Target
            });
        }

        public void Enter(IBaseState fromState)
        {
            _playerController.TargetPlayerCharacter.Crouch(true);
            _animatorController.Play(CAMERA_SHAKER_NAME, CAMERA_SHAKER_CROSS_FADE_DURATION);
        }
        public void Exit(IBaseState toState)
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