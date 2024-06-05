using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct TurretSystem : ISystem
{
    private EntityQuery _query;
    private ComponentTypeHandle<ShootingComponent> _turretHandle;
    private ComponentTypeHandle<ShootingComponentBlob> _blobHandle;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
       // state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
       _query = SystemAPI.QueryBuilder().WithAllRW<ShootingComponent>().WithAll<ActiveTurretComponent, ShootingComponentBlob>().Build();
       _turretHandle = state.GetComponentTypeHandle<ShootingComponent>(false);
       _blobHandle = state.GetComponentTypeHandle<ShootingComponentBlob>(true);
       
       state.RequireForUpdate(_query);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
       // EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
       //     .CreateCommandBuffer(state.WorldUnmanaged);
       
        _turretHandle.Update(ref state);
        _blobHandle.Update(ref state);
        NativeArray<ShootingComponent> shootingComponents = _query.ToComponentDataArray<ShootingComponent>(Allocator.Temp);
        NativeArray<ShootingComponentBlob> blobs = _query.ToComponentDataArray<ShootingComponentBlob>(Allocator.Temp);
        var entities = _query.ToEntityArray(Allocator.Temp);
        
        float deltaTime = SystemAPI.Time.DeltaTime;
        for (var index = 0; index < shootingComponents.Length; index++)
        {
            var shoot = shootingComponents[index];
            float currentTime = shoot.currentTime + deltaTime;

            var g = blobs[index].config.Value;
            if (currentTime >= g.maxTime)
            {
                currentTime = 0;
                
                LocalToWorld firePoint = SystemAPI.GetComponent<LocalToWorld>(shoot.firePoint);
                for (int i = 0; i < g.numProjectile; i++)
                {
                    Entity e = state.EntityManager.Instantiate(shoot.projectile);
    
                    // Generate random angles within the specified accuracy range
                    float anglex = math.radians(UnityEngine.Random.Range(-g.accuracy, g.accuracy));
                    float angley = math.radians(UnityEngine.Random.Range(-g.accuracy, g.accuracy));

                    // Create rotation quaternions from the random angles
                    quaternion randomRotationX = quaternion.AxisAngle(math.up(), anglex);
                    quaternion randomRotationY = quaternion.AxisAngle(math.right(), angley);
                    // Combine the random rotations
                    quaternion combinedRotation = math.mul(randomRotationY, randomRotationX);

                    // Transform the forward vector by the combined random rotations
                    float3 randomDirection = math.mul(combinedRotation, math.forward());

                    // Apply the firePoint rotation to get the final direction
                    float3 direction = math.mul(firePoint.Rotation, randomDirection);
                    direction = math.normalize(direction);

                    // Call your Shoot method with the correct direction and rotation
                    Shoot(ref state, e, firePoint.Position, quaternion.LookRotation(direction, firePoint.Up), direction);
                }
            }

            shoot.currentTime = currentTime;
            state.EntityManager.SetComponentData(entities[index], shoot);
        }
    }

    [BurstCompile]
    void Shoot(ref SystemState state, Entity e, float3 pos, quaternion rot, float3 forward)
    {
        //Entity e = ecb.Instantiate(shoot.ValueRO.projectile);
        

        //ecb.SetComponent(e, new LocalToWorld()
        state.EntityManager.SetComponentData(e, new LocalTransform()
        {
            Position = pos,
            Rotation = rot,
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
            Direction = forward,
            Gravity = x.Gravity,
        });
    }
}
