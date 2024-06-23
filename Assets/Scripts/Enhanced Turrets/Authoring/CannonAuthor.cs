using Enhanced_Turrets.Components;
using ScriptableObjects;
using Unity.Entities;
using UnityEngine;

namespace Enhanced_Turrets.Authoring
{
    public class CannonAuthor : MonoBehaviour
    {
        [SerializeField] private CannonStats stats;
        public CannonStats Stats => stats;
        private class CannonAuthorBaker : Baker<CannonAuthor>
        {
            public override void Bake(CannonAuthor authoring)
            {
                AddComponent(GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic), new EnhancedCannonComponent()
                {
                    Projectile = GetEntity(authoring.stats.Prefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}