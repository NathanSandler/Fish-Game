using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct ImpactComponent : IComponentData
{
    public Entity Prefab;
    public int MaxImpactCount;
}
