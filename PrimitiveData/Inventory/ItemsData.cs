using System;

namespace Data.Inventory
{
    [Serializable]
    public class ItemsArray
    {
        public ItemData[] Items;
    }
    
    [Serializable]
    public class ItemsData
    {
        public ItemsArray ItemsArray;
    }
}