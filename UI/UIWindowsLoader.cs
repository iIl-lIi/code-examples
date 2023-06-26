using Game.UI;
using UnityEngine;

public static class UIWindowsLoader
{
    public static UIWindow Load(UIWindowContainer container, Transform parent)
    {
        if (container.WindowInstance != null) return container.WindowInstance;
        container.WindowInstance = Object.Instantiate(container.Prefab, parent.transform);
        var window = container.WindowInstance.GetComponent<UIWindow>();
        window.Initialize();
        container.WindowInstance = window;
        return window;
    }
    public static bool Unload(UIWindowContainer container)
    {
        if(container.WindowInstance == null) return false;
        Object.Destroy(container.WindowInstance.gameObject);
        return true;
    }
}