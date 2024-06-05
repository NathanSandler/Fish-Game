using System;
using Enhanced_Turrets.Authoring;
using Enhanced_Turrets.Components;
using ScriptableObjects;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class EnhancedTurretAuthor : MonoBehaviour
{
    [SerializeField] private Cannon[] cannons;
    
    [Serializable] 
    private struct Cannon
    {
        public CannonStats stats;
        public CannonAuthor firePoint;
    } 
    private class EnhancedTurretAuthorBaker : Baker<EnhancedTurretAuthor>
    {
        public override void Bake(EnhancedTurretAuthor authoring)
        {
            //Create the turret as an entity
            Entity turret = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            
            var buffer = AddBuffer<EnhancedShootingComponent>(turret);
            //Let's bake each cannon, and add it to our buffer
            foreach (Cannon cannon in authoring.cannons)
            {
                //Bind a dependency for editor purposes
                DependsOn(cannon.stats);

                //Add to blob asset reference... How do we ensure we're not creating a new blob for literally every cannon?
                BlobAssetReference<EnhancedShootingComponentConfig> config;
                using (var shooting = new BlobBuilder(Allocator.Temp))
                {
                    ref EnhancedShootingComponentConfig mcc =
                        ref shooting.ConstructRoot<EnhancedShootingComponentConfig>();
                    mcc.MaxTime = cannon.stats.FireSpeed;
                    mcc.Accuracy = cannon.stats.Accuracy;
                    mcc.NumProjectile = cannon.stats.NumProjectile;
                    config = shooting.CreateBlobAssetReference<EnhancedShootingComponentConfig>(Allocator.Persistent);
                }

                AddBlobAsset(ref config, out var hash);

                Entity cannonEntity = GetEntity(cannon.firePoint.gameObject, TransformUsageFlags.Dynamic);

                //Attach
                buffer.Add(new EnhancedShootingComponent()
                {
                    Cannon = cannonEntity,
                    Config = config
                });
            }
            AddComponent<ActiveTurretComponent>(turret);
        }
    }
}
