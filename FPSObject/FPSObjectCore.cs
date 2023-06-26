using System;
using Cysharp.Threading.Tasks;
using Game.FPSObject.Implementations;
using Game.FPSObject.Other;
using UnityEngine;

namespace Game.FPSObject
{
    public enum FPSObjectShakerType
    {
        Idle,
        Walk,
        Run,
        JumpIn,
        JumpRunIn,
        OnGrounded,
        OnGroundedRun,
        CrouchIdle,
        CrouchMove
    }
    public enum FPSObjectIndex
    {
        BaseFPSObject,
        BaseWeapon
    }
    public enum FPSObjectState
    {
        Busy,
        Taken,
        PuttedAway
    }
    
    public interface IFPSObject
    {
        FPSObjectIndex Type { get; set; }
        FPSObjectState State { get; set; }
        GameObject Root { get; set; }
        GameObject MeshRoot { get; set; }
        void Initialize();
        UniTask TakeUp();
        UniTask PutAway();
    }

    [Serializable]
    public class FPSObjectPrefab
    {
        public FPSObjectIndex _Index;
        public BaseFPSObject _ObjectPrefab;
        public FPSObjectItem _ItemPrefab;
    }
}