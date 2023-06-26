using UnityEngine;

namespace Game.UI
{
    [System.Serializable]
    public class UIWindowContainer
    {        
        [field: SerializeField] public UIWindow Prefab { get; private set; }
        public UIWindow WindowInstance { get; set; }
    }
}