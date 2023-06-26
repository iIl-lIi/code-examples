using System;

namespace Data.Inventory
{
    [Serializable]
    public class ShopItemsArray
    {
        public string[] Items;
    }
    
    [Serializable]
    public class ShopData
    {
        public ShopItemsArray ItemsArray;
    }
}