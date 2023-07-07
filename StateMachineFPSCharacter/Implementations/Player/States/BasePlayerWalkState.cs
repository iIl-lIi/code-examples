using Game.Item;
using Game.PlayerStateMachine;
using OtherTools._3D.TransformAnimatorSystem;

namespace Game.Character.Implementations.Player.States
{
    public class BasePlayerWalkState : IState
    {
        private const string CAMERA_SHAKER_NAME = "WalkCamera";
        private const float CAMERA_SHAKER_CROSS_FADE_DURATION = 0.1f;
        
        public FPSObjectAnimationType FPSObjectAnimation { get; set; } = FPSObjectAnimationType.Walk;

        private readonly BasePlayerController _playerController;
        private readonly TransformAnimator _shaker;
        private bool _allowControlFactor;

        public BasePlayerWalkState(TransformAnimatorsController animatorController, BasePlayerController playerController)
        {
            this._playerController = playerController;
            _shaker = animatorController.GetAnimator(CAMERA_SHAKER_NAME);

            var addTransform = new AddOffset()
            {
                UseParameters = UseTransformParameters.Everything,
                GetPosition = () => _shaker.Target.localPosition,
                GetRotation = () => _shaker.Target.localRotation
            };
            playerController.TargetPlayerCharacter.AddOffsets.Add(addTransform);
        }
        
        public void Enter(IState fromState)
        {
            if (BasePlayerController.IsCrouch)
            {
                _playerController.SwitchState<BasePlayerCrouchMoveState>();
                return;
            }
            
            _shaker.StartAnimation();
            _allowControlFactor = true;
        }
        public void Exit(IState toState)
        {
            _allowControlFactor = false;
            _shaker.StopAnimation();
        }
        public void Update()
        {
            if (BasePlayerController.IsMove == false)
            {
                _playerController.SwitchState<BasePlayerIdleState>();
                return;
            }
            if (BasePlayerController.IsSprint)
            {
                _playerController.SwitchState<BasePlayerRunState>();
                return;
            }
            if (BasePlayerController.IsCrouch)
            {
                _playerController.SwitchState<BasePlayerCrouchMoveState>();
                return;
            }

            var value = _playerController.PlayerMovementInput.StickValue;
            if(_allowControlFactor) _shaker.SetFactorImmediate(value);
            _shaker.SetSpeedFactorImmediate(value);
        }
    }
}