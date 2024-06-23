using UnityEngine;


namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Game/Projectile", fileName = "Projectile", order = 20)]
    public class ProjectileSO : ScriptableObject
    {
        [field: SerializeField] public float Damage { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public float Lifetime { get; private set; } 
        [field: SerializeField] public int Pierce { get; private set; }

        [field:  SerializeField] public GameObject ImpactEffect { get; private set; }
        
        //Can totally also add some UI stuff in here like Cost, Icon and description
    }
}