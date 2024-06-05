using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    private EntityManager _entityManager;
    
    // Start is called before the first frame update
    void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        NativeArray<HealthComponent> health = _entityManager.CreateEntityQuery(typeof(HealthComponent), typeof(PortalComponent)).ToComponentDataArray<HealthComponent>(Allocator.Temp);
        if (health.Length == 0) return;
        healthBar.value = health[0].Health / health[0].MaxHealth;
    }
}
