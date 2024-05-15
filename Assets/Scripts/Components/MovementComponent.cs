using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Components
{
    public struct MovementComponent : IComponentData
    {
        public LocalTransform Target;
        public float3 Direction;
        public float Gravity;
    }

    public struct MovementComponentBlob : IComponentData
    {
        public BlobAssetReference<MovementComponentConfig> config;
    }
    
    public struct MovementComponentConfig : IComponentData
    {
        public float Speed;
    }
}
