using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementMode : MonoBehaviour
{
    internal void EndPlacement()
    {
        throw new NotImplementedException();
    }

    internal void StartPlacement()
    {
        throw new NotImplementedException();
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
