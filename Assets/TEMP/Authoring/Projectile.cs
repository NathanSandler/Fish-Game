using TEMP.Components;
using TEMP.Scriptables;
using Unity.Entities;
using UnityEngine;

namespace TEMP.Authoring
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private ProjectileSO stats;
       
        private class ProjectileBaker : Baker<Projectile>
        {
            public override void Bake(Projectile authoring)
            {
                DependsOn(authoring.stats);
                
                Entity e = GetEntity(authoring, TransformUsageFlags.Dynamic);
                
                AddComponent(e, new DamageComponent()
                {
                    Damage = authoring.stats.Damage,
                });
                
                AddComponent(e, new SpeedComponent()
                {
                    Speed = authoring.stats.Speed,
                });
            }
        }
    }
}