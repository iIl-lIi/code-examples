using Data.Battle;
using Data.Character;
using Data.Inventory;
using UnityEngine;

namespace Data.Editor
{
    [CreateAssetMenu(order = 0, fileName = "DataBaseContainer", menuName = "Data Base/Container")]
    public class DataBaseContainer : ScriptableObject
    {
        public BattlesData BattlesData;
        public CharactersData CharactersData;
        public ItemsData ItemsData;
        public ShopData ShopData;
    }
}