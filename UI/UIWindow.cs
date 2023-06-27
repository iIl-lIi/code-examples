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
        public readonly WeakReference<CanvasGroup> CanvasGroup = new (null);

        private CancellationTokenSource _cancellationTokenSource;

        public virtual void Initialize()
        {
            var canvasGroup = GetComponent<CanvasGroup>();
            CanvasGroup.SetTarget(canvasGroup);
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0;
            State = ElementState.IsHiden;
        }
        public async UniTask Show()
        {
            if (State == ElementState.Busy) return;

            State = ElementState.Busy;   
            if(CanvasGroup.TryGetTarget(out var canvasGroup))
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            _cancellationTokenSource = new CancellationTokenSource();
            StartShowed?.Invoke(this); 

            while (CanvasGroup.TryGetTarget(out canvasGroup) && canvasGroup.alpha < 1)
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested) break;
                canvasGroup.alpha += Time.deltaTime / FadeDuration;
                await UniTask.Yield();
            }

            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            State = ElementState.IsShown;
            EndShowed?.Invoke(this);
        }
        public async UniTask Hide()
        {   
            if (State == ElementState.Busy) return;
            State = ElementState.Busy;
            if(CanvasGroup.TryGetTarget(out var canvasGroup))
            {   
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            _cancellationTokenSource = new CancellationTokenSource();
            StartHide?.Invoke(this);

            while (CanvasGroup.TryGetTarget(out canvasGroup) && canvasGroup.alpha > 0)
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested) break;
                canvasGroup.alpha -= Time.deltaTime / FadeDuration;
                await UniTask.Yield();
            }

            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
            State = ElementState.IsHiden;
            EndHide?.Invoke(this);
        }
        public void ShowImmediate()
        {
            if(CanvasGroup.TryGetTarget(out var canvasGroup))
            {
                canvasGroup.alpha = 1;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
            State = ElementState.IsShown;
            EndShowed?.Invoke(this);
        }
        public void HideImmediate()
        {
            if(CanvasGroup.TryGetTarget(out var canvasGroup))
            {
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
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