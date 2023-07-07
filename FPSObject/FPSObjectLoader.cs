using Game.Item;
using Game.Item.Implementations;
using Game.Item.Item;
using Game.Item.List;
using UnityEngine;

namespace Game.FPSObject.Implementations
{
    public class FPSObjectLoader : MonoBehaviour
    {
        [SerializeField] private ItemsList _Container;
        [SerializeField] private Transform _ObjectsRoot;

        public IFPSObject LoadedObject => _loadedObject.Item1;
        private (BaseFPSObject, BaseItem) _loadedObject;

        public bool TryLoad(ItemIndex fpsObjectIndex, out IFPSObject result)
        {
            if(_loadedObject != default)
            {
                Debug.LogWarning($"[{this.name}]: The loaded object is not empty. Maybe you need to unload it.");
                result = _loadedObject.Item1;
                return false;
            }

            foreach (var item in _Container.List)
            {
                if (item.Index != fpsObjectIndex) continue;
                if (item is not FPSObjectElement fpsObjectElement) continue;
                var fpsObject = Instantiate(fpsObjectElement.ObjectPrefab, _ObjectsRoot);
                FPSObjectsInitializator.Initialize(fpsObject);
                _loadedObject = (fpsObject, item.ItemPrefab);
                result = fpsObject;
                return true;
            }
            result = null;
            return false;
        }
        public bool TryUnload(ItemIndex fpsObjectIndex, out BaseItem itemPrefab)
        {
            itemPrefab = default;
            if (_loadedObject == default) return false;
            Destroy(_loadedObject.Item1.gameObject);
            itemPrefab = _loadedObject.Item2;
            _loadedObject = default;
            return true;
        }
    }
}