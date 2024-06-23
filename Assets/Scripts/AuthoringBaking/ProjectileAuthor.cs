using Components;
using ScriptableObjects;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class ProjectileAuthor : MonoBehaviour
{
    [SerializeField] private ProjectileSO stats;
    
    //[SerializeField] private Transform target;
    private class ProjectileAuthorBaker : Baker<ProjectileAuthor>
    {
        public override void Bake(ProjectileAuthor authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            
            /*
            AddComponent(entity, new MovementComponent()
            {
                //Speed = authoring.speed,
                Gravity = authoring.stats.Gravity
                //Target = authoring.target.position
                
            });
            */
            
            AddComponent(entity, new LifetimeComponent()
            {
                Value = authoring.stats.Lifetime
            });
            
            BlobAssetReference<ProjectileComponentConfig> config;
            using (var movement = new BlobBuilder(Allocator.Temp))
            {
                ref ProjectileComponentConfig mcc = ref movement.ConstructRoot<ProjectileComponentConfig>();
               // mcc.OnHit = GetEntity(authoring.stats.ImpactEffect, TransformUsageFlags.Renderable);
                mcc.MaxImpactCount = authoring.stats.Pierce;
                mcc.Speed = authoring.stats.Speed;
                mcc.Damage = authoring.stats.Damage;
                config = movement.CreateBlobAssetReference<ProjectileComponentConfig>(Allocator.Persistent);
            }
            AddBlobAsset(ref config, out var hash);
            AddComponent(entity, new ProjectileComponentBlob()
            {
                Blob = config,
                OnHit = GetEntity(authoring.stats.ImpactEffect, TransformUsageFlags.Renderable)
            });

            if (authoring.stats.Pierce >= 1)
            {
                AddBuffer<HitList>(entity);
            }
            
            AddComponent<MovementComponent>(entity);

            DependsOn(authoring.stats);
        }
    }
}

public struct HitList : IBufferElementData
{
    public Entity entity;
}
