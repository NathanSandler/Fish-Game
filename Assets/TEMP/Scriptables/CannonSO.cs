using TEMP.Authoring;
using UnityEngine;

namespace TEMP.Scriptables
{
    [CreateAssetMenu(order = 1, fileName = "Cannon", menuName = "FishGame/Cannon")]
    public class CannonSO : ScriptableObject
    {
        [field: Header("Shooting")]
        
        [field: SerializeField] public float SecondBetweenShots { get; private set; }
        [field: SerializeField, Min(1)] public int NumProjectiles { get; private set; }
        [field: SerializeField, Range(0,90)] public float Accuracy { get; private set; }
        [field: SerializeField] public Projectile Projectile { get; private set; }
    }
}