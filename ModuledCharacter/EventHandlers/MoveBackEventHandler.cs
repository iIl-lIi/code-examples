using Data.Character.Animation;
using UnityEngine;

namespace BattleSystem.Character.EventHandlers
{
    public class MoveBackEventHandler : AnimationEventHandler
    {
        private Vector3 currentPosition;
        
        private readonly Transform characterTransform;
        
        public MoveBackEventHandler(AnimationData animationData, 
                                    AnimationsEventsListener listener, 
                                    Transform characterTransform)
                                    : base(animationData, listener)
        {
            this.characterTransform = characterTransform;
            WaitedForEndAnimation += OnWaitedForEndAnimation;
        }

        private void OnWaitedForEndAnimation()
        {
            currentPosition.x = listener.Animator.GetFloat(Constants.CURVE_MOVE_BACK_POSITION);
            characterTransform.position = -currentPosition * characterTransform.localScale.z;
        }
    }
}