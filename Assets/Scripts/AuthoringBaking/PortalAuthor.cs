using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Entities;
using UnityEngine;

public class PortalAuthor : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    private class PortalAuthorBaker : Baker<PortalAuthor>
    {
        public override void Bake(PortalAuthor authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.WorldSpace);
            AddComponent<PortalComponent>(entity);
            AddComponent(entity, new HealthComponent()
            {
                MaxHealth = authoring.maxHealth,
                Health = authoring.maxHealth
            });
        }
    }
}
