using Game.FPSObject.Implementations;
using UnityEngine;

namespace Game.FPSObject.Other
{
    [RequireComponent(typeof(FPSObjectsController))]
    public class FPSObjectsControllerControlsPC : MonoBehaviour
    {
        [SerializeField] private KeyCode _DropKey = KeyCode.Q;
        [SerializeField] private KeyCode[] _SlotsKeys;
        
        private FPSObjectsController _fpsObjectsController;

        private void Awake()
        {
            _fpsObjectsController = GetComponent<FPSObjectsController>();
        }
        private async void Update()
        {
            for (var i = 0; i < _SlotsKeys.Length; i++)
            {
                if (!UnityEngine.Input.GetKeyDown(_SlotsKeys[i])) continue;
                await _fpsObjectsController.TakeUpObject(i);
                break;
            }
        }
        private async void LateUpdate()
        {
            if (UnityEngine.Input.GetKeyDown(_DropKey))
                await _fpsObjectsController.DropObject(_fpsObjectsController.CurrentObjectIndex);
        }
    }
}