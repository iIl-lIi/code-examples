using System;

namespace Data.Inventory
{
    [Serializable]
    public class InventoryItemsArray
    {
        public InventoryItemData[] Items;
    }
    
    [Serializable]
    public class InventoryData
    {
        public InventoryItemsArray ItemsArray;
    }
}