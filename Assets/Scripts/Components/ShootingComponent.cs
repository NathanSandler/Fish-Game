using Unity.Entities;

namespace Components
{
    public struct ShootingComponent : IComponentData
    {
        public float speed;
        public float maxTime;
        public float currentTime;
        public Entity projectile;
        public Entity firePoint;
        // When we do blobs, add back numProjectiles
    }
}