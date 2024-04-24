using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    public partial struct AISystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (aiRO, transformRW) in SystemAPI.Query<RefRO<MovementComponent>, RefRW<LocalTransform>>())
            {
                ref readonly MovementComponent ai = ref aiRO.ValueRO;
                float3 dir = ai.Target - transformRW.ValueRO.Position;
                float dist = math.sqrt(dir.x * dir.x + dir.y * dir.y + dir.z * dir.z);
                if (dist <= 0.5) continue;
                float time = SystemAPI.Time.DeltaTime;
                ref LocalTransform transform = ref transformRW.ValueRW;
                transform.Position += (dir / dist) * (ai.Speed * time);
                transform.Rotation = TransformHelpers.LookAtRotation(transform.Position, ai.Target, transform.Up());
            }
        }
    }
}
