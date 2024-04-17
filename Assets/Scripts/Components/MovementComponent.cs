using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct MovementComponent : IComponentData
    {
        public float Speed;
        public float3 Target;
    }
}
