using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class FishSpawnerAuthor : MonoBehaviour
{
    [SerializeField] private Transform[] transforms;
    [SerializeField] private GameObject prefab;
    [SerializeField] private float radius;
    [SerializeField, Min(0.001f)] private float spawnTime;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var t in transforms)
        {
            Gizmos.DrawWireSphere(t.position, radius);
        }
    }

    private class FishSpawnerAuthorBaker : Baker<FishSpawnerAuthor>
    {
        public override void Bake(FishSpawnerAuthor authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            /*NativeArray<float3> positions = new NativeArray<float3>(authoring.transforms.Length, Allocator.Persistent);
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = authoring.transforms[i].position;
            }*/
            AddComponent(entity, new SpawnComponent()
            {
               // positions = positions,
                prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                offsetRadius = authoring.radius,
                spawnTime = authoring.spawnTime,
            });
            
            DynamicBuffer<SpawnPosition> positionsBuffer = AddBuffer<SpawnPosition>(entity);
            foreach (var t in authoring.transforms)
            {
                positionsBuffer.Add(new SpawnPosition
                {
                    position = t.position
                });
            }
        }
    }
}
