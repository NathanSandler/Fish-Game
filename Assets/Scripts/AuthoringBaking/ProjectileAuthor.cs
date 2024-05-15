using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

public class ProjectileAuthor : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float gravity;
    [SerializeField] private float lifetime;
    
    //[SerializeField] private Transform target;
    private class ProjectileAuthorBaker : Baker<ProjectileAuthor>
    {
        public override void Bake(ProjectileAuthor authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MovementComponent()
            {
                //Speed = authoring.speed,
                Gravity = authoring.gravity
                //Target = authoring.target.position
            });
            AddComponent(entity, new LifetimeComponent()
            {
                Value = authoring.lifetime
            });
            
            BlobAssetReference<MovementComponentConfig> config;
            using (var movement = new BlobBuilder(Allocator.Temp))
            {
                ref MovementComponentConfig mcc = ref movement.ConstructRoot<MovementComponentConfig>();
                mcc.Speed = authoring.speed;
                config = movement.CreateBlobAssetReference<MovementComponentConfig>(Allocator.Persistent);
            }
            AddBlobAsset(ref config, out var hash);
            AddComponent(entity, new MovementComponentBlob()
            {
                config = config
            });
        }
    }
}
