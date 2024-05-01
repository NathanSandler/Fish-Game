using TEMP.Components;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;

namespace TEMP.Systems
{
    //We want to update at the beginning of each tick
    [BurstCompile]
    public partial struct LifetimeSystem : ISystem
    {
        private EntityQuery _query;

        private ComponentTypeHandle<LifeTimeComponent> _lifeTimeHandle;
        private EntityTypeHandle _entityTypeHandle;

        public void OnCreate(ref SystemState state)
        {
            _query = SystemAPI.QueryBuilder().WithAllRW<LifeTimeComponent>().Build();

            _lifeTimeHandle = state.GetComponentTypeHandle<LifeTimeComponent>(false);
            _entityTypeHandle = state.GetEntityTypeHandle();
            
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate(_query);
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            
            _lifeTimeHandle.Update(ref state);
            _entityTypeHandle.Update(ref state);

            state.Dependency = new LifetimeJob()
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                Ecb = ecb.AsParallelWriter(),
                EntityTypeHandle = _entityTypeHandle,
                LifeTimeHandle = _lifeTimeHandle

            }.ScheduleParallel(_query, state.Dependency); //The query restricts how many chunks the job runs in.
        }

        private struct LifetimeJob : IJobChunk // 1024 entites per task?
        {
            public float DeltaTime;
            public EntityCommandBuffer.ParallelWriter Ecb;

            [ReadOnly] public EntityTypeHandle EntityTypeHandle;
            public ComponentTypeHandle<LifeTimeComponent> LifeTimeHandle;
            
            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                NativeArray<LifeTimeComponent> lifeTimeComponents = chunk.GetNativeArray(ref LifeTimeHandle);
                NativeArray<Entity> entities = chunk.GetNativeArray(EntityTypeHandle); // No Ref, because readonly
                
                //Iterate through all entities in chunk

                for (int index = 0; index < chunk.Count; index++)
                {
                    LifeTimeComponent component = lifeTimeComponents[index];
                    component.Value -= DeltaTime;
                    lifeTimeComponents[index] = component;

                    if (component.Value < 0)
                    {
                        //Entity has died
                        Ecb.DestroyEntity(unfilteredChunkIndex, entities[index]);
                    }
                }
            }
        }
    }
}