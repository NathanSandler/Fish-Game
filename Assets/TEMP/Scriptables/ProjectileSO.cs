using UnityEngine;

namespace TEMP.Scriptables
{
    [CreateAssetMenu(order = 1, fileName = "Projectile", menuName = "FishGame/Projectile")]
    public class ProjectileSO : ScriptableObject
    {
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public float Damage { get; private set; }
    }
}