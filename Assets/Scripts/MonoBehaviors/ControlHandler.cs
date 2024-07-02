using UnityEngine;

public static class ControlHandler
{
    private static Controls inputActions;
    private static int scrollMode;
    private static TurretMode turretMode;
    private static PlacementMode placementMode;

    public static void Init()
    {
        
        inputActions = new Controls();
        scrollMode = 1; // 1 is turret, -1 is placement
        inputActions.Turret.Enable();
        inputActions.Permanent.Enable();
        inputActions.Permanent.ChangeMode.performed += ctx => {
            scrollMode = (int)ctx.ReadValue<float>();
            SwapMode();
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
            turretMode.gameObject.SetActive(true);
            
            placementMode.Disable();
            
            inputActions.Turret.Enable();
            inputActions.Placement.Disable();
            turretMode.SetLeftShoot(false);
            turretMode.SetRightShoot(false);
            turretMode.SetLookDirection(Vector2.zero);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (scrollMode == -1)
        {
            turretMode.gameObject.SetActive(false);
            
            placementMode.Enable();
            
            inputActions.Placement.Enable();
            inputActions.Turret.Disable();
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public static void InitTurret(TurretMode mode)
    {
        turretMode = mode;
        inputActions.Turret.Look.performed += ctx => mode.SetLookDirection(ctx.ReadValue<Vector2>());
        inputActions.Turret.ShootL.performed += ctx => mode.SetLeftShoot(ctx.ReadValueAsButton());
        inputActions.Turret.ShootR.performed += ctx => mode.SetRightShoot(ctx.ReadValueAsButton());

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    public static void InitPlacement(PlacementMode mode)
    {
        placementMode = mode;
        /*inputActions.Placement.Place.performed += ctx => {
            if (ctx.ReadValueAsButton()) mode.EnterPlacementMode();
            else mode.ExitPlacementMode();
            };*/

        inputActions.Placement.Place.performed += ctx => mode.OnClick();
        inputActions.Placement.MousePos.performed += ctx => mode.OnMouseMoved(ctx.ReadValue<Vector2>());

        //Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = true;
    }


}
