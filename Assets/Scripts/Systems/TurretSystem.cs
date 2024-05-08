using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct TurretSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
       // state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
       // EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
       //     .CreateCommandBuffer(state.WorldUnmanaged);

        float deltaTime = SystemAPI.Time.DeltaTime;
        foreach(var shoot in SystemAPI.Query<RefRW<ShootingComponent>>())
        {
            float currentTime = shoot.ValueRO.currentTime + deltaTime;

            if (currentTime >= shoot.ValueRO.maxTime)
            {
                currentTime = 0;
                //Entity e = ecb.Instantiate(shoot.ValueRO.projectile);
                Entity e = state.EntityManager.Instantiate(shoot.ValueRO.projectile);
                
                LocalToWorld firePoint = SystemAPI.GetComponent<LocalToWorld>(shoot.ValueRO.firePoint);
                
                //ecb.SetComponent(e, new LocalToWorld()
                state.EntityManager.SetComponentData(e, new LocalTransform()
                {
                    Position = firePoint.Position,
                    Rotation = firePoint.Rotation,
                    Scale = 1
                });
                //ecb.SetComponent(e, new PhysicsVelocity()
                state.EntityManager.SetComponentData(e, new PhysicsVelocity()
                {
                    Linear = firePoint.Forward * SystemAPI.GetComponent<MovementComponent>(e).Speed
                });
                
                
               
            }
            shoot.ValueRW.currentTime = currentTime;
        }
    }
}
