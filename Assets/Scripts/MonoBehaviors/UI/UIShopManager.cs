using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(1000)]
public class UIShopManager : MonoBehaviour
{
    public ShopItemsSO[] shopItems;
    public Button storeButton;
    public RectTransform gridLayout;
        
    // Start is called before the first frame update
    void Start()
    {
        float sizeDelta = storeButton.GetComponent<RectTransform>().sizeDelta.y;
        gridLayout.sizeDelta = new Vector2(gridLayout.sizeDelta.x, sizeDelta + (20 + sizeDelta) * (shopItems.Length - 1));
        
        foreach (ShopItemsSO item in shopItems)
        {
            Button b = Instantiate(storeButton, gridLayout);
            //b.onClick.AddListener(() => PlacementMode.instance.BindTurret(item));
            b.onClick.AddListener(() => print(item));
            
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
