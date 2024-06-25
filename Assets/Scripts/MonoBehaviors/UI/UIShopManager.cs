using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIShopManager : MonoBehaviour
{
    public GameObject[] shopItems;
    public GameObject gridLayout;
        
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i <= shopItems.Length - 1; i++)
        {
            Instantiate(shopItems[i], gridLayout.transform);
        }
    }

    public void Clicked(string itemName)
    {
        Debug.Log("Bought a " + itemName);
    }
}
