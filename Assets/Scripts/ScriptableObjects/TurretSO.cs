using UnityEngine;


namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Game/Turret", fileName = "Turret", order = 0)]
    public class TurretSO : ScriptableObject
    {
        [field: SerializeField] public float fireSpeed { get; private set; }
        //[field: SerializeField] public GameObject projectile { get; private set; }
        [field: SerializeField] public float numProjectile { get; private set; }
        [field: SerializeField] public float accuracy { get; private set; }
        [field: SerializeField] public float rotateSpeed { get; private set; }
    }
}