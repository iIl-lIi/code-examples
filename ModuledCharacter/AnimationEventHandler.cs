using System;
using System.Threading.Tasks;
using Data.Character.Animation;

namespace BattleSystem.Character
{
    public class AnimationEventHandler
    {
        public event Action WaitedForEndAnimation;
        public readonly AnimationData animationData;
        protected readonly AnimationsEventsListener listener;
        protected bool endAnimationEvent;

        public AnimationEventHandler(AnimationData animationData, AnimationsEventsListener listener)
        {
            this.animationData = animationData;
            this.listener = listener;
            listener.Received += OnHandle;
        }
        
        public async Task Execute()
        {
            endAnimationEvent = false;
            listener.Animator.CrossFade(animationData.AnimationName, 0.25f);
            while (!endAnimationEvent)
            {
                WaitedForEndAnimation?.Invoke();
                await Task.Yield();
            }
        }
        
        protected virtual void OnHandle(string eventData)
        {
            if(eventData == Constants.ANIMATION_EVENT_END) 
                endAnimationEvent = true;
        }
    }
}