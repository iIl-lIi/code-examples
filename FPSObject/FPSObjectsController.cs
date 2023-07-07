using UnityEngine;

namespace Game.Item
{
    public class FPSObjectsController : MonoBehaviour
    {
        [SerializeField] private BaseFPSObject TargetObject;
        [SerializeField] private KeyCode _TakeUpKey = KeyCode.None;
        [SerializeField] private KeyCode _PutAwayKey = KeyCode.None;

        public bool IsBusy { get; set; }

        private async void TakeUp()
        {
            if (IsBusy) return;
            IsBusy = true;

            await TargetObject.TakeUp();
            EventsBus.FPSObject.TakenUp.Invoke(TargetObject);
            IsBusy = false;
        }
        private async void PutAway()
        {
            if (IsBusy) return;
            IsBusy = true;

            await TargetObject.PutAway();
            EventsBus.FPSObject.PutedAway.Invoke(TargetObject);
            IsBusy = false;
        }

        private void LateUpdate()
        {
            if (UnityEngine.Input.GetKeyDown(_TakeUpKey))  TakeUp();
            if (UnityEngine.Input.GetKeyDown(_PutAwayKey)) PutAway();
        }
    }
}