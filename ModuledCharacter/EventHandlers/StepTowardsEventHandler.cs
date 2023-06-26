using Data.Character.Animation;
using UnityEngine;

namespace BattleSystem.Character.EventHandlers
{
    public class StepTowardsEventHandler : AnimationEventHandler
    {
        private readonly Vector3 startPosition;
        private Vector3 currentPosition;
        private readonly Transform characterTransform;
        
        public StepTowardsEventHandler(AnimationData animationData, 
                                       AnimationsEventsListener listener, 
                                       Transform characterTransform)
                                       : base(animationData, listener)
        {
            this.characterTransform = characterTransform;
            startPosition = characterTransform.position;
            WaitedForEndAnimation += OnWaitedForEndAnimation;
        }

        private void OnWaitedForEndAnimation()
        {
            currentPosition.x = listener.Animator.GetFloat(Constants.CURVE_STEP_TOWARDS_POSITION);
            characterTransform.position = startPosition + currentPosition * characterTransform.localScale.z;
        }
    }
}