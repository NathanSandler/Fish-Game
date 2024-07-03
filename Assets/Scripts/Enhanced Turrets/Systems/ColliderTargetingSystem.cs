using Components;
using Enhanced_Turrets.Components;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Enhanced_Turrets.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct ColliderTargetingSystem : ISystem
    {
        private EntityQuery _query;
        [ReadOnly] private ComponentLookup<LocalToWorld> transformLookup;
        [ReadOnly] private ComponentLookup<FishComponent> fishLookup;
        private ComponentLookup<AiTurretComponent> turretLookup; 

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
            _query = SystemAPI.QueryBuilder().WithAll<AiTurretComponent>().Build();
            state.RequireForUpdate(_query);
            state.RequireForUpdate<AiTurretComponent>();
            transformLookup = SystemAPI.GetComponentLookup<LocalToWorld>();
            fishLookup = SystemAPI.GetComponentLookup<FishComponent>();
            turretLookup = SystemAPI.GetComponentLookup<AiTurretComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            transformLookup.Update(ref state);
            fishLookup.Update(ref state);
            turretLookup.Update(ref state);

            state.Dependency = new ColliderTargetJob()
            {
                Fish = fishLookup,
                Turret = turretLookup,
                Distances = transformLookup
            }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

            state.Dependency.Complete();
        }
        
        //[BurstCompile]
        private struct ColliderTargetJob : ITriggerEventsJob
        {
            [ReadOnly] public ComponentLookup<FishComponent> Fish;
            public ComponentLookup<AiTurretComponent> Turret;
            [ReadOnly]  public ComponentLookup<LocalToWorld> Distances;

            public void Execute(TriggerEvent triggerEvent)
            {
                Entity turret = Entity.Null;
                Entity fish = Entity.Null;

                if (!Fish.HasComponent(triggerEvent.EntityA) || !Turret.HasComponent(triggerEvent.EntityB)) return;


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
                
                var aiTurretComponent = Turret[turret];
                var currentTarget = aiTurretComponent.Target;
                
                if (currentTarget == Entity.Null || !Distances.HasComponent(currentTarget))
                {
                    aiTurretComponent.Target = fish;
                    Turret[turret] = aiTurretComponent;
                    Debug.Log("Fish was deleted");
                    return;
                }
                float3 targetPosition = Distances[currentTarget].Position;
                float3 fishPosition = Distances[fish].Position;
                float3 turretPosition = Distances[turret].Position;
                float distFish = math.distance(turretPosition, fishPosition);
                float distTarget = math.distance(turretPosition, targetPosition);
                if (distFish < distTarget)
                {
                    aiTurretComponent.Target = fish;
                    Turret[turret] = aiTurretComponent;
                    Debug.Log("new closest target: " + distFish);
                }
            }
        }
        
    }
    
}