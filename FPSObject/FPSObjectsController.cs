using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Character.Implementations.Player;
using Game.FPSObject.Other;
using UnityEngine;

namespace Game.FPSObject.Implementations
{
    public class FPSObjectsController : MonoBehaviour
    {
        public event Action<IFPSObject> TakenUpObject;
        public event Action<IFPSObject> PutAwayObject;
        
        [SerializeField] private FPSObjectsList _Container;
        [SerializeField] private BasePlayerCharacter _BasePlayerCharacter;
        [SerializeField] private Transform _ObjectsRoot;
        [SerializeField] private Transform _DropItemsPoint;
        [SerializeField] [Min(0)] private int _MaxSlotsCount = 4;
        
        public IFPSObject CurrentObject { get; private set; }
        public int CurrentObjectIndex { get; private set; }
        public bool IsBusy { get; set; }
        public int MaxSlotsCount => _MaxSlotsCount;
        private readonly List<IFPSObject> _slots = new();

        public void Initialize()
        {
            IsBusy = false;
            CurrentObject = null;
            CurrentObjectIndex = -1;
        }
        public async UniTask TakeUpObject(int index)
        {
            if (IsBusy || index >= _slots.Count || index < 0) 
                return;
                        
            IsBusy = true;
            if (index == CurrentObjectIndex)
            {
                if (CurrentObject.State == FPSObjectState.Taken)
                { 
                    await CurrentObject.PutAway();
                    PutAwayObject?.Invoke(CurrentObject);
                }
                else if (CurrentObject.State == FPSObjectState.PuttedAway)
                {
                    TakenUpObject?.Invoke(CurrentObject);
                    await CurrentObject.TakeUp();
                }
                IsBusy = false;
                return;
            }

            if (CurrentObject != null && CurrentObject.State == FPSObjectState.Taken)
            {
                await CurrentObject.PutAway();
                PutAwayObject?.Invoke(CurrentObject);
            }
            CurrentObject = _slots[index];
            CurrentObjectIndex = index;
            if (CurrentObject != null)
            {
                TakenUpObject?.Invoke(CurrentObject);
                await CurrentObject.TakeUp();
            }
            IsBusy = false;
        }
        public bool AddFPSObject(FPSObjectIndex fpsObjectIndex)
        {
            if (_slots.Count == MaxSlotsCount) return false;
            foreach (var item in _Container.List)
            {
                if(item._Index != fpsObjectIndex) continue;
                var fpsObject = Instantiate(item._ObjectPrefab, _ObjectsRoot);
                fpsObject.Initialize();
                _slots.Add(fpsObject);
                return true;
            }
            return false;
        }
        public bool RemoveFPSObject(int index)
        {
            if (_slots.Count == 0 || index >= _slots.Count || index < 0) return false;
            var fpsObject = _slots[index];
            if (index == CurrentObjectIndex) CurrentObjectIndex = -1;
            if (fpsObject == null) return false;
            _slots.Remove(fpsObject);
            Destroy(fpsObject.Root);
            return true;
        }
        public async UniTask<bool> DropObject(int index)
        {
            if (_slots.Count == 0 || index >= _slots.Count || index < 0) return false;
            
            foreach (var item in _Container.List)
            {
                if(item._Index != _slots[index].Type) continue;
                if(_slots[index].State == FPSObjectState.Busy) return false;
                await _slots[index].PutAway();
                RemoveFPSObject(index);
                Instantiate(item._ItemPrefab, _DropItemsPoint.position, Quaternion.identity);
                return true;
            }
            return false;
        }

        private void Awake()
        {
            Initialize();
        }
    }
}