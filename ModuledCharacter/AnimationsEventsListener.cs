using System;
using UnityEngine;

namespace BattleSystem.Character
{
    [RequireComponent(typeof(Animator))]
    public class AnimationsEventsListener : MonoBehaviour
    {
        public event Action<string> Received;
        
        public Animator Animator
        {
            get
            {
                if (animator) return animator;
                animator = GetComponent<Animator>();
                return animator;
            }
        }
        
        private Animator animator;

        public void CustomEvent(string data) => Received?.Invoke(data);
        public void EndEvent() => Received?.Invoke(Constants.ANIMATION_EVENT_END);
    }
}