using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

public class ProjectileAuthor : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lifetime;
    //[SerializeField] private Transform target;
    private class ProjectileAuthorBaker : Baker<ProjectileAuthor>
    {
        public override void Bake(ProjectileAuthor authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MovementComponent()
            {
                Speed = authoring.speed,
                //Target = authoring.target.position
            });
            AddComponent(entity, new LifetimeComponent()
            {
                Value = authoring.lifetime
            });
        }
    }
}
