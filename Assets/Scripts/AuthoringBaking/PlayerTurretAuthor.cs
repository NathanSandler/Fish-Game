using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Entities;
using UnityEngine;

public class PlayerTurretAuthor : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float fireSpeed;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform firePoint2;
    [SerializeField] private GameObject projectile;
    [SerializeField] private int numProjectiles;
    private class PlayerTurretAuthorBaker : Baker<PlayerTurretAuthor>
    {
        public override void Bake(PlayerTurretAuthor authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TurretComponent()
            {
                turningSpeed = authoring.rotateSpeed,
                firePoint = GetEntity(authoring.firePoint, TransformUsageFlags.Dynamic),
                firePoint2 = GetEntity(authoring.firePoint2, TransformUsageFlags.Dynamic),
                projectile = GetEntity(authoring.projectile, TransformUsageFlags.Dynamic),
            });
            AddComponent(entity, new ShootingComponent()
            {
                maxTime = authoring.fireSpeed,
                projectile = GetEntity(authoring.projectile, TransformUsageFlags.Dynamic),
                currentTime = 0,
                firePoint = GetEntity(authoring.firePoint, TransformUsageFlags.Dynamic)
            });
            AddComponent(entity, new ControlledTurretComponent()
            {
                e = entity
            });
            AddComponent<ActiveTurretComponent>(entity);
        }
    }
}
