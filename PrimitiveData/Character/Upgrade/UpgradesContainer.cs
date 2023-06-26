using System;

namespace Data.Character.Upgrade
{
    [Serializable]
    public class UpgradesArray
    {
        public UpgradesData[] Upgrades;
    }
    
    [Serializable]
    public class UpgradesContainer
    {
        public UpgradesArray UpgradesArray;
    }
}