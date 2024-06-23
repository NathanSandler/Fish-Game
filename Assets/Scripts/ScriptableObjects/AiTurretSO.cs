using UnityEngine;


namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Game/Turret", fileName = "Turret", order = 0)]
    public class AiTurretSO : ScriptableObject
    {
        [field: SerializeField] public LayerMask TargetLayers { get; private set; }
        [field: SerializeField] public float RotationSpeed { get; private set; }
        [field: SerializeField] public float Range { get; private set; }
        
        //Can totally also add some UI stuff in here like Cost, Icon and description
    }
}