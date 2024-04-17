using Unity.Entities;

namespace Components
{
    public struct HealthComponent : IComponentData
    {
        public float Health;
        public float MaxHealth;
    }
}
