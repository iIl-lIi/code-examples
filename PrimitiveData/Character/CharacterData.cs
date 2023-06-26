using System;
using System.Collections.Generic;
using Data.Character.Animation;

namespace Data.Character
{
    [Serializable]
    public class CharacterData
    {
        public string DataId;
        public string ModelId;
        public string DisplayName;
        public float BattleBarPositionOffset;
        public CharacterParametersData Parameters;
        
        public List<AnimationData> IdleAnimations;
        public List<AnimationData> StepTowardsAnimations;
        public List<AnimationData> MoveBackAnimations;
        public List<AnimationAttackData> AttackAnimations;
        public List<AnimationFactorData> ReceivedDamageAnimations;
        public List<AnimationData> DieAnimations;
        public List<AnimationData> WinAnimations;
        
    }
}