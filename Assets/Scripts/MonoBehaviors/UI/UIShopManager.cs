using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(1000)]
public class UIShopManager : MonoBehaviour
{

    
    public Button storeButton;
    public RectTransform gridLayout;
        
    // Start is called before the first frame update
    void Start()
    {
        float sizeDelta = storeButton.GetComponent<RectTransform>().sizeDelta.y;
        gridLayout.sizeDelta = new Vector2(gridLayout.sizeDelta.x, sizeDelta + (20 + sizeDelta) * (TurretRegisterAuthor.TurretRegisterAuthorBaker.ShopItems.Length - 1));

        for (var index = 0; index < TurretRegisterAuthor.TurretRegisterAuthorBaker.ShopItems.Length; index++)
        {
            ShopItemsSO item = TurretRegisterAuthor.TurretRegisterAuthorBaker.ShopItems[index];
            Button b = Instantiate(storeButton, gridLayout);
            b.onClick.AddListener(() => PlacementMode.Instance.BindTurret(item, index));

            Transform t = b.transform;
            t.GetChild(0).GetComponent<Image>().color = StaticUtilities.colorConversion[item.Type];
            t.GetChild(0).GetChild(0).GetComponent<Image>().sprite = item.Icon;
            t.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.Cost.ToString();
            t.GetChild(2).GetComponent<TextMeshProUGUI>().text = item.name;
        }
    }

    public void Clicked(string itemName)
    {
        Debug.Log("Bought a " + itemName);
    }

    void UpdateMoney()
    {
        
    }

    private void OnEnable()
    {
        
    }
}
