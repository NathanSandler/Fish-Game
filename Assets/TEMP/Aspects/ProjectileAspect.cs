using TEMP.Components;
using Unity.Entities;
using Unity.Transforms;

namespace TEMP.Aspects
{
    public readonly partial struct ProjectileAspect : IAspect
    {
        [Optional] private readonly RefRO<TargetComponent> _target;
        private readonly RefRO<SpeedComponent> _speed;
        private readonly RefRW<LocalTransform> _transform;

        public void Move(float time, ComponentLookup<LocalToWorld> positions)
        {
            if (_target.IsValid && positions.TryGetComponent(_target.ValueRO.Value, out LocalToWorld comp))
            {
                _transform.ValueRW.Rotation = TransformHelpers.LookAtRotation(_transform.ValueRO.Position,
                    comp.Position, _transform.ValueRW.Up());
            }

            _transform.ValueRW.Position += _speed.ValueRO.Speed * time * _transform.ValueRW.Forward();
            
        }
    }
}