using System;

namespace Data.Character.Animation
{
    [Serializable]
    public class AnimationData
    {
        public string AnimationName;
        public float ActivateChance = 1;
        public int Cost;
    }
}