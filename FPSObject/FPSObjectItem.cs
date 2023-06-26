using System;
using UnityEngine;

namespace Game.FPSObject.Other
{
    public class FPSObjectItem : MonoBehaviour
    {
        public event Action<FPSObjectItem> Click;
        
        [SerializeField] private FPSObjectIndex _Index;
        [SerializeField] private float _DestroyDelay;
        public FPSObjectIndex Index => _Index;
        public float DestroyDelay => _DestroyDelay;
    }
}