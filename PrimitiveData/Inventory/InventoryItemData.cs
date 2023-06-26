using System;

namespace Data.Inventory
{
    [Serializable]
    public class InventoryItemData
    {
        public string Id;
        public int Count;
        public bool IsUsed;
    }
}