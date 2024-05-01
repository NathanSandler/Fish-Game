using Unity.Entities;
using Unity.Transforms;

public struct TurretComponent : IComponentData
{
    public Entity firePoint;
    public Entity firePoint2;
    public Entity projectile;
    public float fireSpeed;
    public float turningSpeed;
    public int numProjectiles;
}
