using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Entities;
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
            AddComponent(entity, new HealthComponent()
            {
                Health = stats.Health,
                MaxHealth = stats.Health
            });
            //Entity target = GetEntity(authoring.target, TransformUsageFlags.None);
            AddComponent(entity, new MovementComponent()
            {
                //Speed = stats.Speed,
                //Target = authoring.target.position
            });
            DependsOn(authoring.stats);
        }
    }
}
