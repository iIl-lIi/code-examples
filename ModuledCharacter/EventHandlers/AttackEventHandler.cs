using System;
using Data.Character.Animation;

namespace BattleSystem.Character.EventHandlers
{
    public class AttackEventHandler : AnimationEventHandler
    {
        public readonly bool isLongRangeAttack;
        
        private readonly Action<float> attack;

        public AttackEventHandler(AnimationAttackData animationAttackData,
            AnimationsEventsListener listener, Action<float> attackFunction = default) : 
            base(animationAttackData, listener)
        {
            this.attack = attackFunction;
            isLongRangeAttack = animationAttackData.IsLongRangeAttack;
        }
        
        protected override void OnHandle(string eventData)
        {
            base.OnHandle(eventData);
            if (eventData != animationData.AnimationName) return;
            var attackData = (AnimationAttackData) animationData;
            attack?.Invoke(attackData.Factor);
        }
    }
}