using Unity.Entities;

namespace TEMP.Components
{
    public struct CannonComponent : IComponentData, IEnableableComponent
    {
        public Entity Projectile;
        public Entity FirePoint;
        public float TimeBetweenShots;
        public float Accuracy;
    }
}