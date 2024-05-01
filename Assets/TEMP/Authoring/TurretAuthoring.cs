using TEMP.Components;
using Unity.Entities;
using UnityEngine;

public class TurretAuthoring : MonoBehaviour
{
    [SerializeField] private Cannon[] cannons;
    [SerializeField] private float movementSpeed;

    private class TurretBaker : Baker<TurretAuthoring>
    {
        public override void Bake(TurretAuthoring authoring)
        {

            Entity e = GetEntity(authoring, TransformUsageFlags.Dynamic);
            
            AddComponent(e, new TurretComponent()
            {
                MovementSpeed = authoring.movementSpeed
            });
            
            AddComponent<ShootingComponent>(e);

            //All of the cannons need to turn off if the turret turns off.
            /*
            foreach (Cannon cannon in authoring.cannons)
            {
                Entity c = GetEntity(cannon, TransformUsageFlags.Dynamic);
                
                AddComponent(c, );
            }
            */
            
        }
    }

}
