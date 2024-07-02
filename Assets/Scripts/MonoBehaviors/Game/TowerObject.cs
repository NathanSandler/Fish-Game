using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class TowerObject : MonoBehaviour
{
    private SphereCollider _collider;
    private ShopItemsSO _stats;
    public ShopItemsSO Stats => _stats;
    private Entity _boundEntity;

    
    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
    }

    //Pass down required info... Which well isn't much...
    public void Initialize(Transform binding, ShopItemsSO stats, int index)
    {
        transform.localScale = binding.localScale;
        //Convert the object to an entity. Alternatively, could have a singleton system in ECS and just tell it what tower and where 
        Entity singleton = ObjectPlacer.EntityManager.CreateEntityQuery(typeof(TurretRegisterSingleton)).ToEntityArray(Allocator.Temp)[0];
        ObjectPlacer.EntityManager.SetComponentData(singleton, new TurretRegisterSingleton()
        {
            Location = binding.position,
            TurretID = index
        });
        ObjectPlacer.EntityManager.SetComponentEnabled<TurretRegisterSingleton>(singleton, true);
        
        //_boundEntity = ObjectPlacer.EntityManager.CreateEntity();
        //ObjectPlacer.EntityManager.AddComponentData(_boundEntity, new LocalToWorld { Value = float4x4.TRS(binding.position, binding.rotation, binding.localScale) });
        //ObjectPlacer.EntityManager.AddComponentObject(_boundEntity, binding.gameObject);
        
        _stats = stats;
        _collider.radius = stats.Radius;
        
        //Destroy the old one
        Destroy(binding.gameObject);

    }

    public void OnDestroy()
    {
        // Destroy the entity
        if (ObjectPlacer.EntityManager.Exists(_boundEntity))
        {
            ObjectPlacer.EntityManager.DestroyEntity(_boundEntity);
        }
    }


}
