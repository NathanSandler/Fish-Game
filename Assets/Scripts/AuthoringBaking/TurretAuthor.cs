using Components;
using ScriptableObjects;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class TurretAuthor : MonoBehaviour
{
    [SerializeField] TurretSO stats;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform firePoint2;
    [SerializeField] private GameObject projectile;
    private class TurretAuthorBaker : Baker<TurretAuthor>
    {
        public override void Bake(TurretAuthor authoring)
        {
            DependsOn(authoring.stats);
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new TurretComponent()
            {
                turningSpeed = authoring.stats.rotateSpeed,
                firePoint = GetEntity(authoring.firePoint, TransformUsageFlags.Dynamic),
                firePoint2 = GetEntity(authoring.firePoint2, TransformUsageFlags.Dynamic),
                
            });
            AddComponent(entity, new ShootingComponent()
            {
                currentTime = 0,
                firePoint = GetEntity(authoring.firePoint, TransformUsageFlags.Dynamic),
                projectile = GetEntity(authoring.projectile, TransformUsageFlags.Dynamic)
            });
            AddComponent<ActiveTurretComponent>(entity);
            
            BlobAssetReference<ShootingComponentConfig> config;
            using (var shooting = new BlobBuilder(Allocator.Temp))
            {
                ref ShootingComponentConfig mcc = ref shooting.ConstructRoot<ShootingComponentConfig>();
                mcc.maxTime = authoring.stats.fireSpeed;
                mcc.accuracy = authoring.stats.accuracy;
                mcc.numProjectile = authoring.stats.numProjectile;
                mcc.rotateSpeed = authoring.stats.rotateSpeed;
                config = shooting.CreateBlobAssetReference<ShootingComponentConfig>(Allocator.Persistent);
            }
            AddBlobAsset(ref config, out var hash);
            AddComponent(entity, new ShootingComponentBlob()
            {
                config = config
            });
        }
    }
}
