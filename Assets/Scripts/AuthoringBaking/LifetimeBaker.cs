using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class LifetimeBaker : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    private class LifetimeBakerBaker : Baker<LifetimeBaker>
    {
        public override void Bake(LifetimeBaker authoring)
        {
            AddComponent(GetEntity(authoring.gameObject, TransformUsageFlags.Renderable), new LifetimeComponent()
            {
                Value = authoring.lifeTime
            });
        }
    }
}
