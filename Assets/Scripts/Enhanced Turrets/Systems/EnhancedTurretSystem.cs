using Components;
using Enhanced_Turrets.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(TransformSystemGroup))]
public partial struct EnhancedTurretSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        //Iterate through all turret Objects
        foreach (var (_, entity) in SystemAPI.Query<RefRO<ActiveTurretComponent>>().WithEntityAccess())
        {
            //Update all the cannons

            if (!SystemAPI.IsBufferEnabled<EnhancedShootingComponent>(entity)) continue;

            var buffer = SystemAPI.GetBuffer<EnhancedShootingComponent>(entity);
            
            for (int index = 0; index < buffer.Length; index++)
            {

                var cannon = SystemAPI.GetComponent<EnhancedCannonComponent>(buffer[index].Cannon);
                var shoot = cannon;
                var blob = buffer[index].Config.Value;
                float currentTime = shoot.CurrentTime + deltaTime;
                
                if (currentTime >= blob.MaxTime && SystemAPI.IsComponentEnabled<EnhancedCannonComponent>(buffer[index].Cannon))
                {
                    currentTime = 0;

                    LocalToWorld firePoint = SystemAPI.GetComponent<LocalToWorld>(buffer[index].Cannon);
                    
                    Entity shootFx = state.EntityManager.Instantiate(cannon.ShootEffect);
                    SetDirection(ref state, shootFx, firePoint.Position, quaternion.LookRotation(firePoint.Forward, firePoint.Up));
                    
                    for (int i = 0; i < blob.NumProjectile; i++)
                    {
                        
                        Entity e = state.EntityManager.Instantiate(cannon.Projectile);
                        
                        float2 angle = UnityEngine.Random.insideUnitCircle * math.radians(blob.Accuracy);

                        // Create rotation quaternions from the random angles
                        quaternion randomRotationX = quaternion.AxisAngle(math.up(), angle.x);
                        quaternion randomRotationY = quaternion.AxisAngle(math.right(), angle.y);
                        // Combine the random rotations
                        quaternion combinedRotation = math.mul(randomRotationY, randomRotationX);

                        // Transform the forward vector by the combined random rotations
                        float3 randomDirection = math.mul(combinedRotation, math.forward());

                        // Apply the firePoint rotation to get the final direction
                        float3 direction = math.mul(firePoint.Rotation, randomDirection);
                        direction = math.normalize(direction);
        
                        
                        
                        // Call your Shoot method with the correct direction and rotation
                        SetDirection(ref state, e, firePoint.Position, quaternion.LookRotation(direction, firePoint.Up));
                        Shoot(ref state, e, direction);
                    }
                    
                }

                shoot.CurrentTime = currentTime;
                SystemAPI.SetComponent(buffer[index].Cannon, shoot);
                
            }
        }
    }

    [BurstCompile]
    void Shoot(ref SystemState state, Entity e, float3 forward)
    {

        var x = SystemAPI.GetComponent<ProjectileComponentBlob>(e);
        SystemAPI.SetComponent(e, new MovementComponent()
        {
            Velocity = forward * x.Blob.Value.Speed,

        });
    }

    [BurstCompile]
    void SetDirection(ref SystemState state, Entity e, float3 pos, quaternion rot)
    {
        state.EntityManager.SetComponentData(e, new LocalTransform()
        {
            Position = pos,
            Rotation = rot,
            Scale = 1
        });
    }
}
