using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Entities;
using UnityEngine;

public class PlayerTurretAuthor : MonoBehaviour
{
    
    private class PlayerTurretAuthorBaker : Baker<PlayerTurretAuthor>
    {
        public override void Bake(PlayerTurretAuthor authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new ControlledTurretComponent()
            {
                e = entity
            });
        }
    }
}
