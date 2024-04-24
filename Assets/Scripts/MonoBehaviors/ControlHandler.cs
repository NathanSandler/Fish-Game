using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public static class ControlHandler
{
    private static Controls inputActions;
    private static int scrollMode;
    private static GameObject turretMode;
    private static GameObject placementMode;

    public static void Init()
    {
        
        inputActions = new Controls();
        scrollMode = 1; // 1 is turret, -1 is placement
        inputActions.Turret.Enable();
        inputActions.Permanent.Enable();
        Debug.Log("Init");
        inputActions.Permanent.ChangeMode.performed += ctx => {
            scrollMode = (int)ctx.ReadValue<float>();
            SwapMode();
            Debug.Log("Scroll: " + scrollMode);
            };
        inputActions.UI.PauseGame.performed += _ => ExitPause();
        inputActions.Permanent.PauseGame.performed += _ => EnterPause();
    }

    private static void EnterPause()
    {
        inputActions.Turret.Disable();
        inputActions.Placement.Disable();
        inputActions.Permanent.Disable();
        inputActions.UI.Enable();
    }

    private static void ExitPause()
    {
        SwapMode();
        inputActions.Permanent.Enable();
        inputActions.UI.Disable();
    }

    private static void SwapMode()
    {
        if (scrollMode == 1)
        {
            turretMode.SetActive(true);
            placementMode.SetActive(false);
            inputActions.Turret.Enable();
            inputActions.Placement.Disable();
        }
        else if (scrollMode == -1)
        {
            turretMode.SetActive(false);
            placementMode.SetActive(true);
            inputActions.Placement.Enable();
            inputActions.Turret.Disable();
        }
    }

    public static void InitTurret(TurretMode mode)
    {
        turretMode = mode.gameObject;
        inputActions.Turret.ShootL.performed += ctx => mode.SetLeftShoot(ctx.ReadValueAsButton());
        inputActions.Turret.ShootR.performed += ctx => mode.SetRightShoot(ctx.ReadValueAsButton());
    }

    public static void InitPlacement(PlacementMode mode)
    {
        placementMode = mode.gameObject;
        inputActions.Placement.Place.performed += ctx => {
            if (ctx.ReadValueAsButton()) mode.StartPlacement();
            else mode.EndPlacement();
            };
    }


}
