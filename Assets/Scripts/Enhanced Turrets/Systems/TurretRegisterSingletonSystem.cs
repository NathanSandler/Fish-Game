using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Enhanced_Turrets.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(BeginInitializationEntityCommandBufferSystem))]
    public partial struct TurretRegisterSingletonSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
           state.RequireForUpdate<TurretRegisterSingleton>();
           state.RequireForUpdate<TurretVariants>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            Entity ent = SystemAPI.GetSingletonEntity<TurretVariants>();

            if (!SystemAPI.IsComponentEnabled<TurretRegisterSingleton>(ent)) return;
            
            TurretRegisterSingleton register = SystemAPI.GetComponent<TurretRegisterSingleton>(ent);
            var buffer = SystemAPI.GetBuffer<TurretVariants>(ent);


            //EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            Entity newTurret = state.EntityManager.Instantiate(buffer[register.TurretID].Entity);

            //Entity newTurret  = ecb.Instantiate(buffer[register.TurretID].Entity);
            
            SystemAPI.SetComponent(newTurret, new LocalTransform()
            {
                Position = register.Location
            });
            
            SystemAPI.SetComponentEnabled<TurretRegisterSingleton>(ent, false);

            //ecb.Playback(state.EntityManager);
            //ecb.Dispose();
        }
    }
}