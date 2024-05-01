using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public partial struct LifetimeManagerSystem : ISystem
{
    private EntityQuery _query;
    private ComponentTypeHandle<LifetimeComponent> _lifetimeHandle;
    private EntityTypeHandle _entityHandle;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _query = SystemAPI.QueryBuilder().WithAllRW<LifetimeComponent>().Build();
        _lifetimeHandle = state.GetComponentTypeHandle<LifetimeComponent>(false);
        _entityHandle = state.GetEntityTypeHandle();
        state.RequireForUpdate(_query);
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        
        _lifetimeHandle.Update(ref state);
        _entityHandle.Update(ref state);

        state.Dependency = new lifetimeJob
        {
            ecb = ecb.AsParallelWriter(),
            componentHandle = _lifetimeHandle,
            deltaTime = SystemAPI.Time.DeltaTime,
            entityHandle = _entityHandle
        }.ScheduleParallel(_query, state.Dependency);
    }

    
    private struct lifetimeJob : IJobChunk
    {
        public float deltaTime;
        public EntityCommandBuffer.ParallelWriter ecb;
        [ReadOnly] public EntityTypeHandle entityHandle;
        public ComponentTypeHandle<LifetimeComponent> componentHandle;
        
        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            NativeArray<LifetimeComponent> lifetimeComponents = chunk.GetNativeArray(ref componentHandle);
            NativeArray<Entity> entities = chunk.GetNativeArray(entityHandle);
            for (int i = 0; i < chunk.Count; i++)
            {
                LifetimeComponent current = lifetimeComponents[i];
                current.Value -= deltaTime;
                lifetimeComponents[i] = current;

                if (current.Value < 0)
                {
                    ecb.DestroyEntity(unfilteredChunkIndex, entities[i]);
                }
            }
        }
    }
}
