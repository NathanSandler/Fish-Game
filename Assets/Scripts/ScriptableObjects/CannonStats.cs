using UnityEngine;
using UnityEngine.VFX;


namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Game/Cannon", fileName = "Cannon", order = 0)]
    public class CannonStats : ScriptableObject
    {
        //We can change from GameObject to something more specific if we want.
        [field: Header("Effects")]
        [field: SerializeField] public GameObject ShootEffect { get; private set; }
        
        
        [field: Header("Turret Stats")]
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField] public float FireSpeed { get; private set; }
        [field: SerializeField] public float NumProjectile { get; private set; }
        [field: SerializeField] public float Accuracy { get; private set; }
       
    }
}