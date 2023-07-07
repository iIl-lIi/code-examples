using UnityEngine;

namespace Game.Item.List
{
    [CreateAssetMenu(fileName = "FPSObjectElement", menuName = "Item/List Element/FPS Object Element", order = 0)]
    public class FPSObjectElement : ItemListElement
    {
        [field: SerializeField] public BaseFPSObject ObjectPrefab { get; private set; }
    }
}