using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct SpawnComponent : IComponentData
{
    public Entity prefab;
    //public NativeArray<float3> positions;
    public float offsetRadius;
    public float spawnTime;
    public float currentSpawnTime;
}

public struct SpawnPosition : IBufferElementData
{
    public float3 position;
}
