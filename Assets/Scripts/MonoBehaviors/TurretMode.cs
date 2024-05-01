using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMode : MonoBehaviour
{
    public static Transform instance { get; private set; }

    internal void SetLeftShoot(bool v)
    {
        
    }

    internal void SetRightShoot(bool v)
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        ControlHandler.InitTurret(this);

        if (instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = transform;
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
