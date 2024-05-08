using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(TransformSystemGroup))]
[BurstCompile]
public partial struct TurretSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        float deltaTime = SystemAPI.Time.DeltaTime;
        foreach(var shoot in SystemAPI.Query<RefRW<ShootingComponent>>())
        {
            float currentTime = shoot.ValueRO.currentTime + deltaTime;

            if (currentTime >= shoot.ValueRO.maxTime)
            {
                Entity e = ecb.Instantiate(shoot.ValueRO.projectile);
                //Entity e = state.EntityManager.Instantiate(shoot.ValueRO.projectile);
                LocalToWorld firePoint = SystemAPI.GetComponent<LocalToWorld>(shoot.ValueRW.firePoint);
                ecb.SetComponent(e, firePoint);
                ecb.SetComponent(e, new PhysicsVelocity()
                {
                    Linear = firePoint.Forward * shoot.ValueRO.speed
                });
                
                currentTime = 0;
            }
            shoot.ValueRW.currentTime = currentTime;
        }
    }
}
