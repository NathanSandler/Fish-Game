using Enhanced_Turrets.Components;
using Unity.Entities;
using UnityEngine;

public class AITurretAuthor : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Transform upDownJoint;
    [SerializeField] private Transform leftRightJoint;
    [SerializeField] private Transform eye;
    private class AITurretAuthorBaker : Baker<AITurretAuthor>
    {
        public override void Bake(AITurretAuthor authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new AiTurretComponent
            {
                TurningSpeed =  authoring.rotationSpeed,
                LeftRightRotation = GetEntity(authoring.leftRightJoint, TransformUsageFlags.Dynamic),
                UpDownRotation = GetEntity(authoring.upDownJoint, TransformUsageFlags.Dynamic),
                Eye = GetEntity(authoring.eye, TransformUsageFlags.Dynamic)
            });
        }
    }
}