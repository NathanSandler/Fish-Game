using TEMP.Aspects;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace TEMP.Systems
{
    //Update after transforms have updated so we have accurate locations
    [UpdateAfter(typeof(TransformSystemGroup))]
    [BurstCompile]
    public partial struct ProjectileSystem : ISystem
    {
        private ComponentLookup<LocalToWorld> _positions;

        public void OnCreate(ref SystemState state)
        {
            _positions = SystemAPI.GetComponentLookup<LocalToWorld>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _positions.Update(ref state);
            //Get access to all projectiels

            //Voodoo witchcraft makes it only run on objects containing the voodoo witchcraft
            state.Dependency = new ProjectileMovementJob()
            {
                Positions = _positions,
                DeltaTime = SystemAPI.Time.DeltaTime
            }.ScheduleParallel(state.Dependency);


        }

        public partial struct ProjectileMovementJob : IJobEntity
        {
            public float DeltaTime;
            [ReadOnly] public ComponentLookup<LocalToWorld> Positions;

            private void Execute(ProjectileAspect aspect)
            {
                aspect.Move(DeltaTime, Positions);
            }
        }
    }
}