using Unity.Entities;
using Unity.Transforms;

namespace Enhanced_Turrets.Components
{
    public struct TargetableComponent : IBufferElementData
    {
        public Entity entity;
    }
}