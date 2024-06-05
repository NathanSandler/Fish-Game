using Enhanced_Turrets.Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct EnhancedPlayerTurretControlSystem : ISystem
{

    public void OnUpdate(ref SystemState state)
    {
        foreach (var (transform, _, entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<ControlledTurretComponent>>().WithEntityAccess())
        {
            //transform.ValueRW = transform.ValueRW.RotateY(time * ControlHandler.lookPos.x * stats.ValueRO.turningSpeed);
            //transform.ValueRW = transform.ValueRW.RotateX(time * ControlHandler.lookPos.y * stats.ValueRO.turningSpeed);
            transform.ValueRW.Rotation = TurretMode.PlayerTransform.localRotation;

            //If we get shutdown, then we shouldn't update.
            if (!SystemAPI.IsBufferEnabled<EnhancedShootingComponent>(entity)) continue;
            
            var cannons = SystemAPI.GetBuffer<EnhancedShootingComponent>(entity);

           
            
            if (cannons.Length != 2)
            {
                Debug.LogWarning("Player does not have assumed two cannons, please correct this! " + cannons.Length);
                continue;
            }
            
            SystemAPI.SetComponentEnabled<EnhancedCannonComponent>(cannons[0].Cannon, TurretMode.isShootingLeft);
            SystemAPI.SetComponentEnabled<EnhancedCannonComponent>(cannons[1].Cannon, TurretMode.isShootingRight);
        }
    }
}
