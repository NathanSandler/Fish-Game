using TEMP.Aspects;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

namespace TEMP.Systems
{
    public partial struct TurretSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            PhysicsWorldSingleton physics = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

            
            //Iterate through all living towers... Because towers are IEnable, we have the advantage of those ones not running
            foreach (var VARIABLE in SystemAPI.Query<TurretAspect>())
            {
                
            }
            

        }
    }
}