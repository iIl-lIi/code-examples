using System.Collections.Generic;
using Game.UI;
using UnityEngine;

[CreateAssetMenu(fileName = "WindowContainersList", menuName = "UI/Create containers list", order = 0)]
public class ContainersList : ScriptableObject
{
    [field: SerializeField] public List<UIWindowContainer> List { get; private set; }
}