using Game.UI.Other;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI.Implementations.Windows.Inventory
{
    public class PlayerInventoryWindow : UIWindow
    {
        [Header("Character Raw")]
        [SerializeField] private UIDragable _Dragable;
        [SerializeField] private GameObject _CharacterRoot;
        [SerializeField] private Transform _CharacterCameraRoot;
        [SerializeField] [Min(0)] private float _Sensitivity = 3;

        private Vector3 _characterAngle;
        private bool _updateCharacterRotation;

        public override void Initialize()
        {
            base.Initialize();
            _characterAngle = Vector3.zero;
            _updateCharacterRotation = false;
            
            StartShowed += OnStartShowed;
            EndHide += OnEndHide;
            _Dragable.Drag += OnDrag;
        }
        public void HideInventory() => HideInventoryProcess();
        
        private async void HideInventoryProcess()
        {
            await Cancellation();
            await Hide();
        }
        private void OnDrag(PointerEventData obj)
        {
            if (State != ElementState.IsShown) return;
            var velocity = obj.delta.x;
            if (velocity > _Sensitivity) velocity = _Sensitivity;
            if (velocity < -_Sensitivity) velocity = -_Sensitivity;
            _characterAngle.y += velocity * _Sensitivity * Time.deltaTime;
        }
        private void OnStartShowed(UIWindow window)
        {
            InteractionsBlocking.Block<ShootingBlock>();
            _CharacterCameraRoot.rotation = Quaternion.Euler(_characterAngle);
            _updateCharacterRotation = true;
            _CharacterRoot.SetActive(true);
        }
        private void OnEndHide(UIWindow window)
        {
            InteractionsBlocking.Unblock<ShootingBlock>();
            _CharacterRoot.SetActive(false);
            _updateCharacterRotation = false;
        }

        private void UpdateCharacterRotation()
        {
            if (_updateCharacterRotation == false) return;
            _CharacterCameraRoot.rotation = Quaternion.LerpUnclamped(
                _CharacterCameraRoot.rotation,
                Quaternion.Euler(_characterAngle),
                Time.deltaTime * 16);
        }
        private void Update()
        {
            UpdateCharacterRotation();
        }
        private void OnDestroy()
        {
            StartShowed -= OnStartShowed;
            EndHide -= OnEndHide;
            _Dragable.Drag -= OnDrag;
        }
    }
}