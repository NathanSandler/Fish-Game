using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Components
{
    //When this component is applied, all this data must be defined
    public struct MovementComponent : IComponentData
    {
        public Entity Target;
        public float3 Velocity;
    }
}
