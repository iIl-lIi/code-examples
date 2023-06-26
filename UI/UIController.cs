using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace Game.UI
{
    public static class UIController
    {
        private static RectTransform _root;
        private static ContainersList _windowContainers;

        public static void Initialize(RectTransform root, ContainersList containers)
        {
            _root = root;
            _windowContainers = containers;
            UnloadAllWindows();
            LayoutRebuilder.ForceRebuildLayoutImmediate(root);
        }
        public static void UnloadAllWindows(params UIWindow[] exeptWindows)
        {
            var cont = false;
            foreach (var wc in _windowContainers.List)
            {
                cont = false;
                foreach (var ew in exeptWindows)
                {
                    if(ew != wc.WindowInstance) continue;
                    cont = true;
                    break;
                }
                if(cont || wc.WindowInstance == null) continue;
                UIWindowsLoader.Unload(wc);
            }
        }
        public static void HideAllWindowsImmediate(params UIWindow[] exeptWindows)
        {
            var cont = false;
            foreach (var wc in _windowContainers.List)
            {
                cont = false;
                foreach (var ew in exeptWindows)
                {
                    if(ew != wc.WindowInstance) continue;
                    cont = true;
                    break;
                }
                if(cont || wc.WindowInstance == null) continue;
                wc.WindowInstance.HideImmediate();
            }
        }
        public static async UniTask HideAllWindows(params UIWindow[] exeptWindows)
        {
            var actions = new List<UniTask>();
            var cont = false;
            foreach (var wc in _windowContainers.List)
            {
                cont = false;
                foreach (var ew in exeptWindows)
                {
                    if(ew != wc.WindowInstance) continue;
                    cont = true;
                    break;
                }
                if(cont || wc.WindowInstance == null) continue;
                actions.Add(wc.WindowInstance.Hide());
            }
            await UniTask.WhenAll(actions);
        }

        public static T LoadWindow <T>() where T : UIWindow
        {
            foreach (var wc in _windowContainers.List)
                if (wc.Prefab is T tWindow)
                    return UIWindowsLoader.Load(wc, _root.transform) as T;

            Debug.LogError($"Window '{nameof(T)}' is not exist!");
            return default;
        }
        public static T LoadWindowWithPrefab <T>(T prefab) where T : UIWindow
        {
            foreach (var wc in _windowContainers.List)
                if (wc.Prefab == prefab)
                    return UIWindowsLoader.Load(wc, _root.transform) as T;

            Debug.LogError($"Window '{nameof(T)}' is not exist!");
            return default;
        }
        public static T GetWindow<T>() where T : UIWindow
        {
            foreach (var wc in _windowContainers.List)
                if (wc.WindowInstance is T tWindow)
                    return tWindow;

            Debug.LogError($"Window '{nameof(T)}' is not exist!");
            return default;
        }
        public static T GetWindowWithPrefab<T>(T prefab) where T : UIWindow
        {
            foreach (var wc in _windowContainers.List)
                if (wc.Prefab == prefab)
                    return UIWindowsLoader.Load(wc, _root.transform) as T;

            Debug.LogError($"Window '{nameof(T)}' is not exist!");
            return default;
        }
        public static bool UnloadWindow(UIWindow instance)
        {
            foreach (var wc in _windowContainers.List)
            {
                if (wc.WindowInstance != instance) continue;
                return UIWindowsLoader.Unload(wc);
            }
            return false;
        }
        public static bool UnloadWindow<T>() where T : UIWindow
        {
            foreach (var wc in _windowContainers.List)
            {
                if (wc.WindowInstance is not T tWindow) continue;
                return UIWindowsLoader.Unload(wc);
            }
            return false;
        }
        public static bool UnloadWindowWithPrefab<T>(T prefab) where T : UIWindow
        {
            foreach (var wc in _windowContainers.List)
            {
                if (wc.Prefab != prefab) continue;
                return UIWindowsLoader.Unload(wc);
            }
            return false;
        }
    }
}