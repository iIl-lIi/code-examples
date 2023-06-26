using System;

namespace Data.Character.Upgrade
{
    [Serializable]
    public class UpgradesData
    {
        public string CharacterDataId;
        public int HitPoints;
        public int AttackValue;
        public int WinsCount;
        public int AllowHitPoints;
        public int AllowAttackValue;
    }
}