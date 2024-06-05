using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial struct PortalSystem : ISystem
{
    private EntityQuery _query;
    private EntityTypeHandle _entityHandle;
    private ComponentTypeHandle<LocalToWorld> componentHandle;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _query = SystemAPI.QueryBuilder().WithAll<LocalToWorld, FishComponent>().Build();
        _entityHandle = state.GetEntityTypeHandle();
        componentHandle = state.GetComponentTypeHandle<LocalToWorld>();
        
        state.RequireForUpdate<PortalComponent>();
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        
        Entity entity = SystemAPI.GetSingletonEntity<PortalComponent>();
        float pos = SystemAPI.GetComponent<LocalToWorld>(entity).Position.z;
        
        _entityHandle.Update(ref state);
        componentHandle.Update(ref state);

        state.Dependency = new PortalJob
        {
            ecb = ecb.AsParallelWriter(),
            position = pos,
            entityHandle = _entityHandle,
            componentHandle = componentHandle
        }.ScheduleParallel(_query, state.Dependency);

        foreach (var (health,_) in SystemAPI.Query<RefRW<HealthComponent>, RefRO<PortalComponent>>())
        {
            health.ValueRW.Health -= SystemAPI.Time.DeltaTime;
        }
    }

    [BurstCompile]
    public struct PortalJob : IJobChunk
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        [ReadOnly] public EntityTypeHandle entityHandle;
        [ReadOnly] public ComponentTypeHandle<LocalToWorld> componentHandle;
        public float position;
        
        [BurstCompile]
        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            NativeArray<LocalToWorld> positionComponents = chunk.GetNativeArray(ref componentHandle);
            NativeArray<Entity> entities = chunk.GetNativeArray(entityHandle);
            for (int i = 0; i < chunk.Count; i++)
            {
                if (positionComponents[i].Position.z > position)
                {
                    // TODO: Damage health
                    ecb.DestroyEntity(unfilteredChunkIndex, entities[i]);
                }
            }
        }
    }
}
