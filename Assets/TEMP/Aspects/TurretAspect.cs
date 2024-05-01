using TEMP.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace TEMP.Aspects
{
    public readonly partial struct TurretAspect : IAspect
    {
        //private readonly NativeArray<CannonComponent> _cannons;
        private readonly RefRO<TurretComponent> _turretData;
        private readonly RefRO<LocalToWorld> _turretLocation;


        public void HandleShooting(float deltaTime)
        {
            
        }

    }
}