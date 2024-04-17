using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

public class ProjectileAuthor : MonoBehaviour
{
    static float speed = 2f;
    [SerializeField] private Transform target;
    private class ProjectileAuthorBaker : Baker<ProjectileAuthor>
    {
        public override void Bake(ProjectileAuthor authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MovementComponent()
            {
                Speed = speed,
                Target = authoring.target.position
            });
            DependsOn(authoring.target);
        }
    }
}
