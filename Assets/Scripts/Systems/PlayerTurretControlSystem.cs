using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct PlayerTurretControlSystem : ISystem
{
    private ComponentLookup<ActiveTurretComponent> shootInput;
    public void OnCreate(ref SystemState state)
    {
        shootInput = state.GetComponentLookup<ActiveTurretComponent>();
    }

    public void OnUpdate(ref SystemState state)
    {
        shootInput.Update(ref state);
        foreach (var (transform, turret) in
                 SystemAPI.Query<RefRW<LocalTransform>, RefRO<ControlledTurretComponent>>())
        {
            //transform.ValueRW = transform.ValueRW.RotateY(time * ControlHandler.lookPos.x * stats.ValueRO.turningSpeed);
            //transform.ValueRW = transform.ValueRW.RotateX(time * ControlHandler.lookPos.y * stats.ValueRO.turningSpeed);
            transform.ValueRW.Rotation = TurretMode.PlayerTransform.localRotation;
            
            shootInput.SetComponentEnabled(turret.ValueRO.rightArm, TurretMode.isShootingRight);
        }
    }
}
