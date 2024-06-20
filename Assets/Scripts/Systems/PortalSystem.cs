using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

public partial struct PortalSystem : ISystem
{
    private EntityQuery _query;
    private EntityTypeHandle _entityHandle;
    private ComponentTypeHandle<LocalToWorld> componentHandle;
    private NativeQueue<uint> portalQueue;
    private NativeReference<uint> portalReference;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _query = SystemAPI.QueryBuilder().WithAll<LocalToWorld, FishComponent>().Build();
        _entityHandle = state.GetEntityTypeHandle();
        componentHandle = state.GetComponentTypeHandle<LocalToWorld>();

        portalQueue = new NativeQueue<uint>(Allocator.Persistent);
        portalReference = new NativeReference<uint>(Allocator.Persistent);
        
        state.RequireForUpdate<PortalComponent>();
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        portalQueue.Dispose();
        portalReference.Dispose();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        portalQueue.Clear();
        portalReference.Value = 0;
        
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
            componentHandle = componentHandle,
            PortalQueue = portalQueue.AsParallelWriter()
        }.ScheduleParallel(_query, state.Dependency);

        state.Dependency = new PortalQueueJob()
        {
            portalQueue = portalQueue,
            portalReference = portalReference
        }.Schedule(state.Dependency);
        
        state.Dependency.Complete();

        Entity portalEntity = SystemAPI.GetSingletonEntity<PortalComponent>();
        HealthComponent healthComponent = SystemAPI.GetComponent<HealthComponent>(portalEntity);
        healthComponent.Health -= portalReference.Value;
        SystemAPI.SetComponent(portalEntity, healthComponent);
    }

    [BurstCompile]
    public struct PortalQueueJob : IJob
    {
        public NativeQueue<uint> portalQueue;
        public NativeReference<uint> portalReference;
        
        [BurstCompile]
        public void Execute()
        {
            while (portalQueue.TryDequeue(out uint damaged))
            {
                portalReference.Value += damaged;
            }
        }
    }

    [BurstCompile]
    public struct PortalJob : IJobChunk
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        [ReadOnly] public EntityTypeHandle entityHandle;
        [ReadOnly] public ComponentTypeHandle<LocalToWorld> componentHandle;
        public float position;

        public NativeQueue<uint>.ParallelWriter PortalQueue;
        
        [BurstCompile]
        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            NativeArray<LocalToWorld> positionComponents = chunk.GetNativeArray(ref componentHandle);
            NativeArray<Entity> entities = chunk.GetNativeArray(entityHandle);
            uint damageTaken = 0;
            
            for (int i = 0; i < chunk.Count; i++)
            {
                if (positionComponents[i].Position.z > position)
                {
                    // TODO: Damage health
                    damageTaken += 1;
                    ecb.DestroyEntity(unfilteredChunkIndex, entities[i]);
                }
            }

            if (damageTaken > 0)
            {
                PortalQueue.Enqueue(damageTaken);
            }
        }
    }
}
