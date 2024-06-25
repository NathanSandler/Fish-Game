using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementMode : MonoBehaviour
{
    internal void ExitPlacementMode()
    {
        
    }

    internal void EnterPlacementMode()
    {
        
        // Show UI to place stuff
        // Stop placement
    }

    // Start is called before the first frame update
    void Start()
    {
        ControlHandler.InitPlacement(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
