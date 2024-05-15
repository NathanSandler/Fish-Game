using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct TurretSystem : ISystem
{
    private EntityQuery _query;
    private ComponentTypeHandle<ShootingComponent> _turretHandle;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
       // state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
       _query = SystemAPI.QueryBuilder().WithAllRW<ShootingComponent>().WithAll<ActiveTurretComponent>().Build();
       _turretHandle = state.GetComponentTypeHandle<ShootingComponent>(false);
       state.RequireForUpdate(_query);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
       // EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
       //     .CreateCommandBuffer(state.WorldUnmanaged);
       
        _turretHandle.Update(ref state);
        NativeArray<ShootingComponent> shootingComponents = _query.ToComponentDataArray<ShootingComponent>(Allocator.Temp);
        float deltaTime = SystemAPI.Time.DeltaTime;
        for (var index = 0; index < shootingComponents.Length; index++)
        {
            var shoot = shootingComponents[index];
            float currentTime = shoot.currentTime + deltaTime;

            if (currentTime >= shoot.maxTime)
            {
                currentTime = 0;
                
                //Entity e = ecb.Instantiate(shoot.ValueRO.projectile);
                Entity e = state.EntityManager.Instantiate(shoot.projectile);

                LocalToWorld firePoint = SystemAPI.GetComponent<LocalToWorld>(shoot.firePoint);

                //ecb.SetComponent(e, new LocalToWorld()
                state.EntityManager.SetComponentData(e, new LocalTransform()
                {
                    Position = firePoint.Position,
                    Rotation = firePoint.Rotation,
                    Scale = 0.2f
                });
                //ecb.SetComponent(e, new PhysicsVelocity()
                /*state.EntityManager.SetComponentData(e, new PhysicsVelocity()
                {
                    Linear = firePoint.Forward * SystemAPI.GetComponent<MovementComponent>(e).Speed
                });*/

                MovementComponent x = SystemAPI.GetComponent<MovementComponent>(e);
                state.EntityManager.SetComponentData(e, new MovementComponent()
                {
                    Direction = firePoint.Forward,
                    Gravity = x.Gravity,
                });
            }

            shoot.currentTime = currentTime;
            shootingComponents[index] = shoot;
        }
    }
}
