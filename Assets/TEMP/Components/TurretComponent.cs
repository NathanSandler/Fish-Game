using Unity.Entities;

namespace TEMP.Components
{
    public struct TurretComponent : IComponentData, IEnableableComponent
    {
        public float MovementSpeed;
    }
}