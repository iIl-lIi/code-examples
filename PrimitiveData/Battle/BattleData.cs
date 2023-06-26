using System;

namespace Data.Battle
{
    [Serializable]
    public class BattleData
    {
        public string MapId;
        public string HeroDataId;
        public string EnemyDataId;
        public int UpgradeHitPoints;
        public int UpgradeAttackValue;
        public RewardData CurrencyReward;
        public RewardData ItemReward;
    }
}