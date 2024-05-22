using Unity.Entities;

namespace Components
{
    public struct ShootingComponent : IComponentData
    {
        public float currentTime;
        public Entity firePoint;
        public Entity projectile;
        // When we do blobs, add back numProjectiles
    }

    public struct ShootingComponentBlob : IComponentData
    {
        public BlobAssetReference<ShootingComponentConfig> config;
    }

    public struct ShootingComponentConfig : IComponentData
    {
        
        public float numProjectile;
        public float accuracy;
        public float maxTime; 
        public float rotateSpeed;
    }
}