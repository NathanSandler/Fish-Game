using Unity.Entities;

public struct ControlledTurretComponent : IComponentData, IEnableableComponent
{
    public Entity rightArm;
    public Entity leftArm;
}
