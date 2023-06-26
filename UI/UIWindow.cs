using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.UI
{
    public enum ElementState { Busy, IsShown, IsHiden} 

    [RequireComponent(typeof(CanvasGroup))]
    public class UIWindow : MonoBehaviour
    {
        public event Action<UIWindow> StartShowed;
        public event Action<UIWindow> StartHide;
        public event Action<UIWindow> EndShowed;
        public event Action<UIWindow> EndHide;

        [field: SerializeField, Min(float.MinValue)] public float FadeDuration { get; private set; } = 0.25f;

        public ElementState State { get; private set; } = ElementState.IsHiden;
        public CanvasGroup CanvasGroup { get; private set; }

        private CancellationTokenSource _cancellationTokenSource;

        public virtual void Initialize()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            CanvasGroup.alpha = 0;
            State = ElementState.IsHiden;
        }
        public async UniTask Show()
        {
            if (State == ElementState.Busy) return;

            State = ElementState.Busy;   
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            _cancellationTokenSource = new CancellationTokenSource();
            StartShowed?.Invoke(this); 

            while (CanvasGroup.alpha < 1)
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested) break;
                CanvasGroup.alpha += Time.deltaTime / FadeDuration;
                await UniTask.Yield();
            }

            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
            State = ElementState.IsShown;
            EndShowed?.Invoke(this);
        }
        public async UniTask Hide()
        {   
            if (State == ElementState.Busy) return;
            State = ElementState.Busy;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            _cancellationTokenSource = new CancellationTokenSource();
            StartHide?.Invoke(this);

            while (CanvasGroup.alpha > 0)
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested) break;
                CanvasGroup.alpha -= Time.deltaTime / FadeDuration;
                await UniTask.Yield();
            }

            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
            State = ElementState.IsHiden;
            EndHide?.Invoke(this);
        }
        public void ShowImmediate()
        {
            CanvasGroup.alpha = 1;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
            State = ElementState.IsShown;
            EndShowed?.Invoke(this);
        }
        public void HideImmediate()
        {
            CanvasGroup.alpha = 0;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            State = ElementState.IsHiden;
            EndHide?.Invoke(this);
        }
        public async UniTask Cancellation()
        {
            if(_cancellationTokenSource == null) return;
            _cancellationTokenSource.Cancel();
            while(_cancellationTokenSource != null)
                await UniTask.Yield();
        }
        public void SetAsLastSibling() => transform.SetAsLastSibling();
    }
}