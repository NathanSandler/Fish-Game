using Unity.Entities;
using UnityEngine;

public class TurretAuthor : MonoBehaviour
{
    private class TurretAuthorBaker : Baker<TurretAuthor>
    {
        public override void Bake(TurretAuthor authoring)
        {
        }
    }
}
