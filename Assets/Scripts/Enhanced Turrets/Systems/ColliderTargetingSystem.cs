using Components;
using Enhanced_Turrets.Components;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Enhanced_Turrets.Systems
{
    [BurstCompile]
    public partial struct ColliderTargetingSystem : ISystem
    {
        private EntityQuery _query;
        [ReadOnly] private ComponentLookup<LocalToWorld> transformLookup;
        [ReadOnly] private ComponentLookup<FishComponent> fishLookup;
        [ReadOnly] private ComponentLookup<AiTurretComponent> turretLookup;
        [ReadOnly]  private BufferLookup<TargetableComponent> targetLookup;
        private EntityTypeHandle _entityHandle;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
            _query = SystemAPI.QueryBuilder().WithAll<AiTurretComponent>().Build();
            state.RequireForUpdate(_query);
            _entityHandle = state.GetEntityTypeHandle();
            transformLookup = SystemAPI.GetComponentLookup<LocalToWorld>();
            fishLookup = SystemAPI.GetComponentLookup<FishComponent>();
            turretLookup = SystemAPI.GetComponentLookup<AiTurretComponent>();
            targetLookup = SystemAPI.GetBufferLookup<TargetableComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            transformLookup.Update(ref state);
            fishLookup.Update(ref state);
            turretLookup.Update(ref state);
            targetLookup.Update(ref state);
            _entityHandle.Update(ref state);

            state.Dependency = new ColliderTargetJob()
            {
                targets = targetLookup,
                Fish = fishLookup,
                Turret = turretLookup
            }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

            state.Dependency.Complete();
            
            state.Dependency = new DistanceCheckJob()
            {
                targets = targetLookup,
                entityHandle = _entityHandle,
                Positions = transformLookup,
                Turrets = turretLookup
            }.ScheduleParallel(_query, state.Dependency);
            
        }
        
        [BurstCompile]
        private struct ColliderTargetJob : ITriggerEventsJob
        {
            [ReadOnly]  public BufferLookup<TargetableComponent> targets;
            [ReadOnly] public ComponentLookup<FishComponent> Fish;
            [ReadOnly] public ComponentLookup<AiTurretComponent> Turret;
            
            public void Execute(TriggerEvent triggerEvent)
            {
                Entity turret = Entity.Null;
                Entity fish = Entity.Null;

                if(!Fish.HasComponent(triggerEvent.EntityA) || !Turret.HasComponent(triggerEvent.EntityB)) return;
                
                
                if (!Fish.HasComponent(triggerEvent.EntityA))
                {
                    turret = triggerEvent.EntityA;
                    fish = triggerEvent.EntityB;
                }
                else
                {
                    fish = triggerEvent.EntityA;
                    turret = triggerEvent.EntityB;
                }

                targets[turret].Add(new TargetableComponent()
                {
                    entity = fish
                });
            }
        }

        [BurstCompile]
        private struct DistanceCheckJob : IJobChunk
        {
            [ReadOnly] public ComponentLookup<LocalToWorld> Positions;
            [ReadOnly] public ComponentLookup<AiTurretComponent> Turrets;
            [ReadOnly]  public BufferLookup<TargetableComponent> targets;
            [ReadOnly] public EntityTypeHandle entityHandle;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                //NativeArray<LifetimeComponent> lifetimeComponents = chunk.GetNativeArray(ref componentHandle);
                NativeArray<Entity> entities = chunk.GetNativeArray(entityHandle);
                foreach (Entity e in entities)
                {
                    var turrets = Turrets[e];
                    float range = turrets.Range;
                    var buffer = targets[e];
                    float3 turretLocation = Positions[e].Position;
                    float currentBest = float.MaxValue;
                    int bestFish = -1;

                    for (var index = 0; index < buffer.Length; index++)
                    {
                        var fish = buffer[index];
                        Entity entity = fish.entity;
                        float3 fishLocation = Positions[entity].Position;
                        float dist = math.distance(turretLocation, fishLocation);
                        if (dist > range)
                        {
                            targets[e].RemoveAt(index);
                            continue; // Redo for-loop
                        }

                        if (dist < currentBest)
                        {
                            currentBest = dist;
                            bestFish = index;
                        }
                    }

                    if (bestFish != -1) turrets.Target = buffer[bestFish].entity;
                }
            }
        }
    }
    
}