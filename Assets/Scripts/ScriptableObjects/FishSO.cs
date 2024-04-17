using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Fish", fileName = "Fish", order = 0)]
public class FishSO : ScriptableObject
{
    [field: SerializeField]
    public float Health { get; private set; }
    
    [field: SerializeField]
    public float Speed { get; private set; }
}
