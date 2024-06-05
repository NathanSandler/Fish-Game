using Unity.Entities;

namespace Enhanced_Turrets.Components
{
    public struct ActiveTurretComponent : IComponentData, IEnableableComponent
    {

    }

//Disabling this disables the turrets rotation
    public struct AiTurretComponent : IComponentData, IEnableableComponent
    {
        public float TurningSpeed;
    }

    public struct ControlledTurretComponent : IComponentData, IEnableableComponent
    {
    }
}

