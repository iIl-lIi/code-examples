using System;
using Cysharp.Threading.Tasks;
using Game.FPSObject.Implementations;
using UnityEngine;

namespace Game.FPSObject.Other
{
    [RequireComponent(typeof(FPSObjectsController))]
    public class FPSObjectItemsDetector : MonoBehaviour
    {
        public event Action<FPSObjectItem> Detected;
        
        [SerializeField] private Transform _DetectionRayOrigin;
        [SerializeField] private LayerMask _DetectionRayMask;
        [SerializeField] [Min(0)] private float _DetectionRayDist;

        public FPSObjectItem CurrentFPSObjectItem { get; private set; }
        public bool IsActive { get; private set; }

        private RaycastHit _hit;
        private FPSObjectsController _fpsObjectsController;

        private bool CastRay() => _DetectionRayOrigin != null && Physics.Raycast(
            _DetectionRayOrigin.position,
            _DetectionRayOrigin.forward,
            out _hit, _DetectionRayDist,
            _DetectionRayMask);
        private void Awake()
        {
            IsActive = false;
            _fpsObjectsController = GetComponent<FPSObjectsController>();
        }
        private async void Update()
        {
            await UniTask.Delay(250);
            CastRay();
            CurrentFPSObjectItem = _hit.collider ? _hit.collider.GetComponent<FPSObjectItem>() : null;
#if UNITY_DEBUG
            if (_DetectionRayOrigin)
            {
                var pos = CurrentFPSObjectItem ? _hit.point
                    : _DetectionRayOrigin.position + _DetectionRayOrigin.forward * _DetectionRayDist;
                var color = CurrentFPSObjectItem ? Color.green : Color.yellow;
                Debug.DrawLine(_DetectionRayOrigin.position, pos, color);
            }
#endif
            if (CurrentFPSObjectItem == null) return;
            Detected?.Invoke(CurrentFPSObjectItem);
        }
        private void LateUpdate()
        {
            if (CurrentFPSObjectItem == null 
                || UnityEngine.Input.GetKeyDown(KeyCode.E) == false
                || _fpsObjectsController.AddFPSObject(CurrentFPSObjectItem.Index) == false) 
                return;
            
            var item = CurrentFPSObjectItem.gameObject;
            item.SetActive(false);
            Destroy(item, CurrentFPSObjectItem.DestroyDelay);
            CurrentFPSObjectItem = null;
        }
    }
}