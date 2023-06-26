using System;

namespace Data.Battle
{
    [Serializable] public class BattlesArray
    {
        public BattleData[] Battles;
    }
    
    [Serializable] public class BattlesData
    {
        public BattlesArray BattlesArray;
    }
}