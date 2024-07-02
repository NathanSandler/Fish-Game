using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlacementAuthor : MonoBehaviour
{
    public EnhancedTurretAuthor[] turrets;
    private class PlacementAuthorBaker : Baker<PlacementAuthor>
    {
        public override void Bake(PlacementAuthor authoring)
        {
            
            
            
        }
    }
}
