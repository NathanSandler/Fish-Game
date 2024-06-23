using Unity.Entities;
using Unity.Mathematics;

namespace Enhanced_Turrets.Components
{
    public struct ActiveTurretComponent : IComponentData, IEnableableComponent
    {

    }

//Disabling this disables the turrets rotation
    public struct AiTurretComponent : IComponentData, IEnableableComponent
    {
        public float TurningSpeed;
        public Entity LeftRightRotation;
        public Entity UpDownRotation;
        public Entity Eye;
    }

    public struct ControlledTurretComponent : IComponentData, IEnableableComponent
    {
    }
}

