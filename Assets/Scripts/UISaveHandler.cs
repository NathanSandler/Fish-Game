using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class UISaveHandler : MonoBehaviour
{
    private async void Start()
    {
        await SaveManager.SaveGame("1");
        print(await SaveManager.LoadSave("1"));
    }
}
