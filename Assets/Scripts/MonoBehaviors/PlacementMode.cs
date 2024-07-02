using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ObjectPlacer))]
public class PlacementMode : MonoBehaviour
{
    public static PlacementMode Instance { get; private set; }
    [SerializeField] private GameObject storePanel;
    
    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;
    [SerializeField] float transitionTime;
    [SerializeField] AnimationCurve curve;

    private int _index;
    private ShopItemsSO _stats;
    private ObjectPlacer _placer;
    
    public void Disable()
    {
        UnBindTurret();
        Lower();
        _placer.enabled = false;
    }

    public void Enable()
    {
        Rise();
      
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        ControlHandler.InitPlacement(this);
        _placer = GetComponent<ObjectPlacer>();
        storePanel.SetActive(false);
        _placer.enabled = false;
    }
    
    public void BindTurret(ShopItemsSO objectStats, int index)
    {
        _stats = objectStats;
        _index = index;
        Debug.Log("Binding Turret: " + objectStats.name);
        Lower(); 
        _placer.BindObject(Instantiate(objectStats.Prefab), objectStats.Radius);
    }

    public void UnBindTurret()
    {
        _stats = null;
        _placer.ClearObject();
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
            //Theres a problem with this function; If OnClick runs immediately before we do stats, then major sadness.
            if (_stats) TryPlaceTurret();
            else InspectTurret();
            


            //Debug.Log("Inject a new actual turret into the ECS world?");
            //UnBindTurret();
            //Rise();
        }

        private void TryPlaceTurret()
        {
            Debug.Log("Trying to place turret");

            //From here, we need to raycast.
            if(_placer.PlaceObject(_stats, _index))   
                _stats = null;
        }

        private void InspectTurret()
        {
            Debug.Log("Trying to inspect turret");

        }

        public void OnMouseMoved(Vector2 readValue)
        {
            _placer.MousePos = readValue;
        }
}
