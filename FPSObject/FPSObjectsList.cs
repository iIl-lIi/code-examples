using System.Collections.Generic;
using UnityEngine;

namespace Game.FPSObject.Other
{
    [CreateAssetMenu(menuName = "FPS/Objects List", fileName = "FPSObjectsList", order = 0)]
    public class FPSObjectsList : ScriptableObject
    {
        [field: SerializeField] public List<FPSObjectPrefab> List { get; private set; }
    }
}