using TEMP.Authoring;
using TEMP.Components;
using TEMP.Scriptables;
using Unity.Entities;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private CannonSO stats;

    private class CannonBaker : Baker<Cannon>
    {
        public override void Bake(Cannon authoring)
        {
            DependsOn(authoring.stats);

            Entity e = GetEntity(authoring, TransformUsageFlags.Dynamic);
            
            AddComponent(e, new CannonComponent()
            {
                Accuracy = authoring.stats.Accuracy,
                TimeBetweenShots = authoring.stats.SecondBetweenShots,
                FirePoint = GetEntity(authoring.firePoint, TransformUsageFlags.Dynamic),
                Projectile = GetEntity(authoring.stats.Projectile, TransformUsageFlags.Dynamic)
            });

        }
    }
}
