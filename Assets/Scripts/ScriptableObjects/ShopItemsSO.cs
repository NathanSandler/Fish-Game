using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/ShopItem", fileName = "ShopItem", order = 30)]

public class ShopItemsSO : ScriptableObject
{
    [field:SerializeField] public Sprite Icon { get; private set; }
    [field:SerializeField] public Transform Prefab { get; private set; }
    [field:SerializeField] public int Cost { get; private set; }
    [field:SerializeField] public float Radius { get; private set; }
    [field:SerializeField] public TurretType Type { get; private set; }
}

public enum TurretType
{
    Laser,
    Explosive
}
