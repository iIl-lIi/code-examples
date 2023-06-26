using Game.FPSObject;
using Game.StateMachine.Player;
using OtherTools._3D.TransformAnimatorSystem;

namespace Game.Character.Implementations.Player.States
{
    public class BasePlayerRunState : IBaseState
    {
        private const string CAMERA_SHAKER_NAME = "RunCamera";
        private const float CAMERA_SHAKER_CROSS_FADE_DURATION = 0.2f;
        
        public FPSObjectShakerType FPSObjectShaker { get; set; } = FPSObjectShakerType.Run;

        private readonly BasePlayerController _playerController;
        private readonly TransformAnimatorsController _animatorController;

        public BasePlayerRunState(TransformAnimatorsController animatorController, BasePlayerController playerController)
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
            _animatorController.Play(CAMERA_SHAKER_NAME, CAMERA_SHAKER_CROSS_FADE_DURATION);
        }
        public void Exit(IBaseState toState)
        {
            _animatorController.Stop(CAMERA_SHAKER_NAME, CAMERA_SHAKER_CROSS_FADE_DURATION);
        }
        public void Update()
        {
            if (BasePlayerController.IsSprint == false || BasePlayerController.IsCrouch)
                _playerController.SwitchState<BasePlayerWalkState>();
        }
    }
}