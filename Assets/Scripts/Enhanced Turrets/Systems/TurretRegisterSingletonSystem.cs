using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Enhanced_Turrets.Systems
{
    [BurstCompile]
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
            
            
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            Entity newTurret  = ecb.Instantiate(buffer[register.TurretID].Entity);
            
            SystemAPI.SetComponent(newTurret, new LocalTransform()
            {
                Position = register.Location
            });
        }
    }
}