using Unity.Entities;
using Unity.Transforms;

public struct TurretComponent : IComponentData
{
    public Entity firePoint;
    public Entity firePoint2;
    public Entity projectile;
    public float turningSpeed;
}
