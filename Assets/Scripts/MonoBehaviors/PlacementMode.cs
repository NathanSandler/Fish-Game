using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlacementMode : MonoBehaviour
{
    public static PlacementMode instance { get; private set; }
    [SerializeField] private GameObject storePanel;
    
    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;
    [SerializeField] float transitionTime;
    [SerializeField] AnimationCurve curve;

    private Camera cam;
    private Transform placementObject;
    private ShopItemsSO stats;
    private Vector2 mousePos;
    
    public void OnDisable()
    {
        //UnBindTurret();
    }

    public void OnEnable()
    {
        Rise();

        // Show UI to place stuff
        // Stop placement
    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        ControlHandler.InitPlacement(this);
        cam = Camera.main;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (stats == null)
        {
            return;
        }
        Debug.DrawRay(cam.ScreenPointToRay(mousePos).origin, cam.ScreenPointToRay(mousePos).direction * 1000f, Color.red);

        if (!Physics.Raycast(cam.ScreenPointToRay(mousePos), out RaycastHit hit, 1000f, StaticUtilities.placableLayer))
        {
            return;
        }

        placementObject.position = hit.point;

        if (Physics.CheckSphere(hit.point, stats.Radius, StaticUtilities.nonPlacableLayer))
        {
            return;
        }
    }

    public void BindTurret(ShopItemsSO stats)
    {
        this.stats = stats;
        placementObject = Instantiate(stats.Prefab);
        Lower(); ;
    }

    public void UnBindTurret()
    {
        stats = null;
        placementObject = null;
        Rise();
    }

    private IEnumerator MovePopup(Vector3 a, Vector3 b, bool isOn)
    {
        Transform popup = storePanel.transform;
        float currentTime = 0;
        if (isOn) storePanel.SetActive(true);
        while (currentTime <= transitionTime)
        {
            currentTime += Time.deltaTime;
            float percent = currentTime / transitionTime;
            popup.position = Vector3.Lerp(a, b, curve.Evaluate(percent));
            yield return null;
        }
        if (!isOn) storePanel.SetActive(false);
        popup.position = Vector3.Lerp(a, b, curve.Evaluate(1));
    }

    public void Rise()
        {
            StopAllCoroutines();
            StartCoroutine(MovePopup(pointA.position, pointB.position, true));
        }

        public void Lower()
        {
            StartCoroutine(MovePopup(pointB.position, pointA.position, false));

        }

        public void OnClick()
        {
            UnBindTurret();
        }

        public void OnMouseMoved(Vector2 readValue)
        {
            mousePos = readValue;
        }

        private void OnDrawGizmos()
        {
            if (stats == null) return;
            if (!Physics.Raycast(cam.ScreenPointToRay(mousePos), out RaycastHit hit, 1000f,
                    StaticUtilities.placableLayer))
            {
                Gizmos.DrawWireSphere(hit.point, stats.Radius);
            }
        }
}
