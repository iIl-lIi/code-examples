using System;

namespace Data.Inventory
{
    [Serializable]
    public class ItemData
    {
        public string Id;
        public string DisplayName;
        public string Description;
        public float ReloadDuration;
        public int Cost;
        public int WinsToUnlock;
    }
}