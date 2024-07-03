using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class TurretRegisterAuthor : MonoBehaviour
{
    //How do we get this out?
    [SerializeField] private ShopItemsSO[] ShopItemsSo;
    

    public class TurretRegisterAuthorBaker : Baker<TurretRegisterAuthor>
    {
        public static ShopItemsSO[] ShopItems { get; private set; }
        public override void Bake(TurretRegisterAuthor authoring)
        {
            ShopItems = authoring.ShopItemsSo;
            Entity e = GetEntity(authoring.gameObject, TransformUsageFlags.None);
            AddComponent<TurretRegisterSingleton>(e);
            SetComponentEnabled<TurretRegisterSingleton>(e, false);

            
            var buffer = AddBuffer<TurretVariants>(e);
            foreach (var shopItem in authoring.ShopItemsSo)
            {
                Entity turret = GetEntity(shopItem.Prefab, TransformUsageFlags.Dynamic);
                buffer.Add(new TurretVariants(){Entity = turret});
                Debug.Log("Added Turret Variant: " + shopItem);
            }
        }
    }
}

public struct TurretRegisterSingleton : IComponentData, IEnableableComponent
{
    public int TurretID;
    public float3 Location;
    public float Scale;
}

[InternalBufferCapacity(16)]
public struct TurretVariants : IBufferElementData
{
    public Entity Entity;
}

