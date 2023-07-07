using Cysharp.Threading.Tasks;
using OtherTools._3D.TransformAnimatorSystem;
using UnityEngine;

namespace Game.Item
{
    public interface IFPSObject
    {
        ItemIndex Index { get; set; }
        FPSObjectState FPSState { get; set; }
        TransformAnimatorFollower AnimatorsFollower { get; set; }
        TransformAnimatorsController AnimatorsController { get; set; }
        GameObject Root { get; set; }
        GameObject MeshRoot { get; set; }
        void Initialize();
        void UpdateFPSObject();
        UniTask TakeUp();
        UniTask PutAway();
    }
}