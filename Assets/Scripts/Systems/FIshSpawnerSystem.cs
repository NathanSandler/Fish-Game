using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct FishSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
        //    .CreateCommandBuffer(state.WorldUnmanaged);
        
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (spawnComponent, spawnPositions) in SystemAPI.Query<RefRW<SpawnComponent>, DynamicBuffer<SpawnPosition>>())
        {
            spawnComponent.ValueRW.currentSpawnTime += deltaTime;
            while (spawnComponent.ValueRO.currentSpawnTime >= spawnComponent.ValueRO.spawnTime)
            {
                spawnComponent.ValueRW.currentSpawnTime -= spawnComponent.ValueRO.spawnTime; // Reset spawn time

                Entity spawnedEntity = ecb.Instantiate(spawnComponent.ValueRO.prefab);
                
                float3 pos = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Length)].position +
                             (float3)UnityEngine.Random.insideUnitSphere * spawnComponent.ValueRO.offsetRadius;
                
                ecb.SetComponent(spawnedEntity, new LocalTransform
                {
                    Position = pos,
                    Rotation = quaternion.identity,
                    Scale = 1f
                });
            }
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}