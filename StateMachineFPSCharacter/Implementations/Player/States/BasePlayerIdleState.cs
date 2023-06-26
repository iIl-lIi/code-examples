using Game.FPSObject;
using Game.StateMachine.Player;
using OtherTools._3D.TransformAnimatorSystem;

namespace Game.Character.Implementations.Player.States
{
    public class BasePlayerIdleState : IBaseState
    {
        private const string CAMERA_SHAKER_NAME = "IdleCamera";
        private const float CAMERA_SHAKER_CROSS_FADE_DURATION = 0.1f;
        
        public FPSObjectShakerType FPSObjectShaker { get; set; } = FPSObjectShakerType.Idle;
        
        private readonly TransformAnimatorsController _animatorController;

        public BasePlayerIdleState(TransformAnimatorsController animatorController, BasePlayerController playerController)
        {
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
        }
    }
}