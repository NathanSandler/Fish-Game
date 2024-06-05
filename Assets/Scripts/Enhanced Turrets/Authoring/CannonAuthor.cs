using Enhanced_Turrets.Components;
using Unity.Entities;
using UnityEngine;

namespace Enhanced_Turrets.Authoring
{
    public class CannonAuthor : MonoBehaviour
    {
        [SerializeField] private ProjectileAuthor projectile;
        private class CannonAuthorBaker : Baker<CannonAuthor>
        {
            public override void Bake(CannonAuthor authoring)
            {
                AddComponent(GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic), new EnhancedCannonComponent()
                {
                    Projectile = GetEntity(authoring.projectile, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}