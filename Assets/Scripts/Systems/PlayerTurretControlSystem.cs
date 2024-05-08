using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct PlayerTurretControlSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (transform, turret) in
                 SystemAPI.Query<RefRW<LocalTransform>, RefRO<ControlledTurretComponent>>())
        {
            //transform.ValueRW = transform.ValueRW.RotateY(time * ControlHandler.lookPos.x * stats.ValueRO.turningSpeed);
            //transform.ValueRW = transform.ValueRW.RotateX(time * ControlHandler.lookPos.y * stats.ValueRO.turningSpeed);
            transform.ValueRW.Rotation = TurretMode.PlayerTransform.localRotation;
        }
    }
}
