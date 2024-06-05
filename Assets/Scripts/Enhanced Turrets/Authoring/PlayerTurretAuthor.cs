using Enhanced_Turrets.Components;
using Unity.Entities;
using UnityEngine;

public class PlayerTurretAuthor : MonoBehaviour
{
    private class PlayerTurretAuthorBaker : Baker<PlayerTurretAuthor>
    {
        public override void Bake(PlayerTurretAuthor authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<ControlledTurretComponent>(entity);
        }
    }
}
