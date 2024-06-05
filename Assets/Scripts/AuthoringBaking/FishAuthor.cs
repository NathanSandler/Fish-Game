using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class FishAuthor : MonoBehaviour
{
    [SerializeField] private FishSO stats;
    [SerializeField] private Transform target;
    private class FishAuthorBaker : Baker<FishAuthor>
    {
        public override void Bake(FishAuthor authoring)
        {
            FishSO stats = authoring.stats;
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<FishComponent>(entity);
            AddComponent(entity, new HealthComponent()
            {
                Health = stats.Health,
                MaxHealth = stats.Health
            });
            //Entity target = GetEntity(authoring.target, TransformUsageFlags.None);
            AddComponent(entity, new MovementComponent()
            {
                Target = GetEntity(authoring.target, TransformUsageFlags.Dynamic),
                Direction = authoring.transform.forward
            });
            DependsOn(authoring.stats);
            
            BlobAssetReference<MovementComponentConfig> config;
            using (var movement = new BlobBuilder(Allocator.Temp))
            {
                ref MovementComponentConfig mcc = ref movement.ConstructRoot<MovementComponentConfig>();
                mcc.Speed = stats.Speed;
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
