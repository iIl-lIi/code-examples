using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Other
{
    [RequireComponent(typeof(Button))]
    public class UIControllerButton : MonoBehaviour
    {
        public enum ControlleMethod 
        {
            HideAllWindows, 
            ShowWindow, 
            HideWindow, 
            ShowWindowImmediate, 
            HideWindowImmediate 
        }

        [SerializeField] private UIWindow TargetWindow;
        [SerializeField] private ControlleMethod _ControlleMethod;

        public Button Button { get; private set; }

        private void Start()
        {
            Button = GetComponent<Button>();
            Button.onClick.AddListener(Click);
        }

        private void Click() => GetMethod(_ControlleMethod)?.Invoke();

        private Action GetMethod(ControlleMethod managerMethod)
        {
            return managerMethod switch
            {
                ControlleMethod.HideAllWindows => ControlleHideAllWindows,
                ControlleMethod.ShowWindow => ControllerShowWindow,
                ControlleMethod.HideWindow => ControllerHideWindow,
                ControlleMethod.ShowWindowImmediate => ControllerShowWindowImmediate,
                ControlleMethod.HideWindowImmediate => ControllerHideWindowImmediate,
                _ => ControlleHideAllWindows
            };
        }

        private async void ControllerShowWindow()     => await UIController.GetWindowWithPrefab(TargetWindow).Show();
        private async void ControllerHideWindow()     => await UIController.GetWindowWithPrefab(TargetWindow).Hide();
        private void ControllerShowWindowImmediate()  =>       UIController.GetWindowWithPrefab(TargetWindow).ShowImmediate();
        private void ControllerHideWindowImmediate()  =>       UIController.GetWindowWithPrefab(TargetWindow).HideImmediate();
        private async void ControlleHideAllWindows()  => await UIController.HideAllWindows();

        private void OnDestroy()
        {
            Button.onClick.RemoveListener(Click);
        }
    }
}