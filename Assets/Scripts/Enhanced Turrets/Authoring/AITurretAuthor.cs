using Enhanced_Turrets.Components;
using Unity.Entities;
using UnityEngine;

public class AITurretAuthor : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Transform upDownJoint;
    [SerializeField] private Transform leftRightJoint;
    private class AITurretAuthorBaker : Baker<AITurretAuthor>
    {
        public override void Bake(AITurretAuthor authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new AiTurretComponent
            {
                TurningSpeed =  authoring.rotationSpeed
            });
        }
    }
}