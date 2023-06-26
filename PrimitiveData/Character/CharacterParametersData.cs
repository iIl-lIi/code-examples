using System;

namespace Data.Character
{
    [Serializable]
    public class CharacterParametersData
    {
        public int HitPoints;
        public int AttackValue;
        public float AttackFactor = 1.0f;
        public float CriticalAttackChance = 0.25f;
        public float CriticalAttackFactor = 1.0f;
        public float ReceivedDamageFactor = 1.0f;
    }
}