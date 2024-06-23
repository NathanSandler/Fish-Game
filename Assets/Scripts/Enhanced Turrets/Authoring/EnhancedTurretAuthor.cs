using Enhanced_Turrets.Authoring;
using Enhanced_Turrets.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class EnhancedTurretAuthor : MonoBehaviour
{
    private class EnhancedTurretAuthorBaker : Baker<EnhancedTurretAuthor>
    {
        public override void Bake(EnhancedTurretAuthor authoring)
        {
            //Create the turret as an entity
            Entity turret = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            
            var buffer = AddBuffer<EnhancedShootingComponent>(turret);
            //Let's bake each cannon, and add it to our buffer
            foreach (var cannon in authoring.GetComponentsInChildren<CannonAuthor>())
            {
                //Bind a dependency for editor purposes
                DependsOn(cannon.Stats);

                //Add to blob asset reference... How do we ensure we're not creating a new blob for literally every cannon?
                BlobAssetReference<EnhancedShootingComponentConfig> config;
                using (var shooting = new BlobBuilder(Allocator.Temp))
                {
                    ref EnhancedShootingComponentConfig mcc =
                        ref shooting.ConstructRoot<EnhancedShootingComponentConfig>();
                    mcc.MaxTime = cannon.Stats.FireSpeed;
                    mcc.Accuracy = cannon.Stats.Accuracy;
                    mcc.NumProjectile = cannon.Stats.NumProjectile;
                    config = shooting.CreateBlobAssetReference<EnhancedShootingComponentConfig>(Allocator.Persistent);
                }

                AddBlobAsset(ref config, out var hash);

                Entity cannonEntity = GetEntity(cannon.gameObject, TransformUsageFlags.Dynamic);

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
