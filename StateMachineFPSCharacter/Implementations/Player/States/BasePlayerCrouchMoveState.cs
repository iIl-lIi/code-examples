using Game.FPSObject;
using Game.StateMachine.Player;
using OtherTools._3D.TransformAnimatorSystem;

namespace Game.Character.Implementations.Player.States
{
    public class BasePlayerCrouchMoveState : IBaseState
    {
        private const string CAMERA_SHAKER_NAME = "CrouchMoveCamera";
        private const float CAMERA_SHAKER_CROSS_FADE_DURATION = 0.1f;
        
        public FPSObjectShakerType FPSObjectShaker { get; set; } = FPSObjectShakerType.CrouchMove;
        
        private readonly BasePlayerController _playerController;
        private readonly TransformAnimator _shaker;
        private bool _allowControlFactor;

        public BasePlayerCrouchMoveState(TransformAnimatorsController animatorController, BasePlayerController playerController)
        {
            this._playerController = playerController;
            _shaker = animatorController.GetShaker(CAMERA_SHAKER_NAME);
            
            playerController.TargetPlayerCharacter.AddTransforms.Add(new AddTransform()
            {
                UseParameters = UseTransformParameters.Everything,
                Transform = _shaker.Target
            });
        }

        public void Enter(IBaseState fromState)
        {
            _playerController.TargetPlayerCharacter.Crouch(true);
            _shaker.StartAnimation();
            _allowControlFactor = true;
        }
        public void Exit(IBaseState toState)
        {
            _allowControlFactor = false;
            _shaker.StopAnimation();
            if (toState is BasePlayerCrouchIdleState) return;
            _playerController.TargetPlayerCharacter.Crouch(false);
        }
        public void Update()
        {
            if (BasePlayerController.IsCrouch && BasePlayerController.CrouchMovement == false)
            {
                _playerController.SwitchState<BasePlayerCrouchIdleState>();
                return;
            }
            if (BasePlayerController.IsCrouch == false)
            {
                if(BasePlayerController.IsSprint) _playerController.SwitchState<BasePlayerRunState>();
                else _playerController.SwitchState<BasePlayerWalkState>();
                return;
            }
            
            var value = _playerController.PlayerMovementInput.StickValue;
            if(_allowControlFactor) _shaker.SetFactorImmediate(value);
            _shaker.SetSpeedFactorImmediate(value);
        }
        
        private void OnEnterSetFactor()
        {
            _allowControlFactor = true;
        }
    }
}