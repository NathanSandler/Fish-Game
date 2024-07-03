using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Enhanced_Turrets.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(BeginFixedStepSimulationEntityCommandBufferSystem))]
    public partial struct TurretRegisterSingletonSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
           state.RequireForUpdate<TurretRegisterSingleton>();
           state.RequireForUpdate<TurretVariants>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            Entity ent = SystemAPI.GetSingletonEntity<TurretVariants>();
            
            // Ensure the entity exists and is valid
            if (!SystemAPI.IsComponentEnabled<TurretRegisterSingleton>(ent)) return;
            SystemAPI.SetComponentEnabled<TurretRegisterSingleton>(ent, false);

            TurretRegisterSingleton register = SystemAPI.GetComponent<TurretRegisterSingleton>(ent);
            var buffer = SystemAPI.GetBuffer<TurretVariants>(ent);
            

            Entity newTurret = state.EntityManager.Instantiate(buffer[register.TurretID].Entity);

            
            SystemAPI.SetComponent(newTurret, new LocalTransform()
            {
                Position = register.Location,
                Rotation = quaternion.identity,
                Scale = register.Scale
            });
        }
    }
}