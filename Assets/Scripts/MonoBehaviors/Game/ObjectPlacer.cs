using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Material = UnityEngine.Material;
using RaycastHit = UnityEngine.RaycastHit;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private TowerObject prefab;
    [SerializeField] private Material highLightMaterial;
    [SerializeField] private Color validOrSelected;
    [SerializeField] private Color invalid;
    
    private Transform _placementObject;
    private Camera _cam;
    private float _radius;

    [NonSerialized] public Vector2 MousePos;
    public bool CanPlace { get; private set; }

    private MeshRenderer[] _meshRenderers;

    private static readonly int HighlightColor = Shader.PropertyToID("_Color");

    public static EntityManager EntityManager {get; private set;}


    private void Awake()
    {
        EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void Start()
    {
        _cam = Camera.main;
    }

    public void BindObject(Transform placement, float radius)
    {
        if(_placementObject) Destroy(_placementObject.gameObject);
        _placementObject = placement;
        enabled = true;
        _meshRenderers = placement.GetComponentsInChildren<MeshRenderer>();
        _radius = radius;
        Debug.Log("Object Bound");
    }

    public bool PlaceObject(ShopItemsSO stats, int index)
    {
        if (!CanPlace) return false;
        
        TowerObject newTower = Instantiate(prefab, _placementObject.position, Quaternion.identity, transform);
        newTower.Initialize(_placementObject, stats, index);
        _placementObject = null;
        enabled = false;
        //Place object into ECS world
        return true;
    }

    public void ClearObject()
    {
        if(_placementObject) Destroy(_placementObject.gameObject);
        _placementObject = null;
        enabled = false;
    }
    

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(_cam.ScreenPointToRay(MousePos).origin, _cam.ScreenPointToRay(MousePos).direction * 1000f, Color.red);

        if (!Physics.Raycast(_cam.ScreenPointToRay(MousePos), out RaycastHit hit, 1000f, StaticUtilities.placableLayer))
        {
            UpdatePlacementState(false);
            return;
        }

        _placementObject.position = hit.point;

        if (Physics.CheckSphere(hit.point, _radius, StaticUtilities.nonPlacableLayer))
        {
            UpdatePlacementState(false);
            return;
        }
        UpdatePlacementState(true);
    }

    private void UpdatePlacementState(bool state)
    {
        if (CanPlace == state) return;
        CanPlace = state;
        
        //Add the shared highlight material from all MeshRenderers
        //Remove the shared highlight material from all MeshRenderers
        //if(CanPlace)
        Color current = CanPlace ? validOrSelected : invalid;
        highLightMaterial.SetColor(HighlightColor, current);
        List<Material> mats = new();
        foreach (MeshRenderer mr in _meshRenderers)
        {
            mr.GetSharedMaterials(mats);
            if (CanPlace) mats.Add(highLightMaterial);
            else mats.RemoveAt(mats.Count-1);
            mr.SetSharedMaterials(mats);
        }
    }

    private void OnDrawGizmos()
    {
        if(!_cam) _cam = Camera.current;
        if (!Physics.Raycast(_cam.ScreenPointToRay(MousePos), out RaycastHit hit, 1000f, StaticUtilities.placableLayer))
        {
            Gizmos.DrawWireSphere(hit.point, _radius);
        }
    }
}
