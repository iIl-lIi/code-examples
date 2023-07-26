using Game.Item;
using Game.PlayerStateMachine;
using OtherTools._3D.TransformAnimatorSystem;

namespace Game.Character.Implementations.Player.States
{
    public abstract class AbstractPlayerState : IState
    {
        public abstract string CameraAnimationName { get; set; }
        public abstract float AnimationCrossFadeDuration { get; set; }
        public abstract FPSObjectAnimationType FPSObjectAnimation { get; set; }
        
        protected readonly TransformAnimatorsController animatorController;
        protected readonly BasePlayerController playerController;
        protected readonly TransformAnimator cameraAnimator;

        public AbstractPlayerState(BasePlayerController playerController)
        {
            this.animatorController = playerController.AnimatorsController;
            this.playerController = playerController;
            cameraAnimator = animatorController.GetAnimator(CameraAnimationName);
            AddAnimationToPlayerCamera(cameraAnimator);
        }

        protected void AddAnimationToPlayerCamera(TransformAnimator cameraAnimator)
        {
            if (cameraAnimator == null) return;

            playerController.TargetPlayerCharacter.AddOffsets.Add(new 
            (
                CameraAnimationName, 
                UseTransformParameters.Everything,
                () => cameraAnimator.Target.localPosition,
                () => cameraAnimator.Target.localRotation
            ));
        }
        
        public virtual void Enter(IState fromState)
        {
            if (string.IsNullOrEmpty(CameraAnimationName)) return;
            animatorController.Play(CameraAnimationName, AnimationCrossFadeDuration);
        }
        public virtual void Exit(IState toState)
        {
            if (string.IsNullOrEmpty(CameraAnimationName)) return;
            animatorController.Stop(CameraAnimationName, AnimationCrossFadeDuration);
        }
        public virtual void Update() { }
    }
}
