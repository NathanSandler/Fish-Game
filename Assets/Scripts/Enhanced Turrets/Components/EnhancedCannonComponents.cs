using Unity.Entities;

namespace Enhanced_Turrets.Components
{
// InternalBufferCapacity specifies how many elements a buffer can have before
// the buffer storage is moved outside the chunk.
//If this component is disabled, the cannons are shutdown
    [InternalBufferCapacity(6)]
    public struct EnhancedShootingComponent : IBufferElementData, IEnableableComponent
    {
        public Entity Cannon;
        public BlobAssetReference<EnhancedShootingComponentConfig> Config;
    }

//if this is disabled, it just means the cannons are not trying to be fired.
    public struct EnhancedCannonComponent : IComponentData, IEnableableComponent
    {
        public float CurrentTime;
        public Entity Projectile;
    }

    public struct EnhancedShootingComponentConfig : IComponentData
    {
        public float NumProjectile;
        public float Accuracy;
        public float MaxTime;
        
    }
}