using Enhanced_Turrets.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Enhanced_Turrets.Systems
{
   [BurstCompile]
    public partial struct AutoTargetingSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;

            foreach (var (turret, entity) in SystemAPI.Query<RefRO<AiTurretComponent>>().WithEntityAccess())
            {
                //Job here
                
                if (!state.EntityManager.Exists(turret.ValueRO.Target))
                {
                    foreach (var shootingComponent in SystemAPI.GetBuffer<EnhancedShootingComponent>(entity))
                    {
                        SystemAPI.SetComponentEnabled<EnhancedCannonComponent>(shootingComponent.Cannon, false);
                    }
                    
                    continue;
                }
                
                var value = turret.ValueRO;
                var horizontal = SystemAPI.GetComponent<LocalTransform>(value.LeftRightRotation);
                var vertical = SystemAPI.GetComponent<LocalTransform>(value.UpDownRotation);
                var eye = SystemAPI.GetComponent<LocalToWorld>(value.Eye);

                // Assume target at (0,0,0)
                float3 target = SystemAPI.GetComponent<LocalToWorld>(value.Target).Position;
                float3 origin = eye.Position;

                // Calculate direction to target from turret base
                float3 direction = math.normalize(target -origin);

                // Calculate desired horizontal rotation (around Y axis)
                float desiredYaw = math.atan2(direction.x, direction.z);
                quaternion targetHorizontalRotation = quaternion.RotateY(desiredYaw);

                // Apply horizontal rotation
                horizontal.Rotation = math.slerp(horizontal.Rotation, targetHorizontalRotation, dt * value.TurningSpeed);
                SystemAPI.SetComponent(value.LeftRightRotation, horizontal);

                // Calculate desired vertical rotation (around X axis)
                float3 localTarget = math.mul(math.inverse(horizontal.Rotation), direction);
                float desiredPitch = math.atan2(-localTarget.y, localTarget.z);
                quaternion targetVerticalRotation = quaternion.RotateX(desiredPitch);

                // Apply vertical rotation
                vertical.Rotation = math.slerp(vertical.Rotation, targetVerticalRotation, dt * value.TurningSpeed);
                SystemAPI.SetComponent(value.UpDownRotation, vertical);

                // Check if the target is within sight
                bool inSight = math.dot(math.normalize(eye.Forward), direction) > 0.9f;
                foreach (var shootingComponent in SystemAPI.GetBuffer<EnhancedShootingComponent>(entity))
                {
                    SystemAPI.SetComponentEnabled<EnhancedCannonComponent>(shootingComponent.Cannon, inSight);
                }
            }
        }
    }
}
