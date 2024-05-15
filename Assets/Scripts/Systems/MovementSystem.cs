using Components;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public partial struct MovementSystem : ISystem
    {
        private EntityQuery _query;
        private ComponentTypeHandle<MovementComponent> componentHandler;
        private ComponentTypeHandle<MovementComponentBlob> blobHandler;
        private ComponentTypeHandle<LocalTransform> transformHandler;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _query = SystemAPI.QueryBuilder().WithAll<MovementComponent, MovementComponentBlob>().WithAllRW<LocalTransform>().Build();
            componentHandler = state.GetComponentTypeHandle<MovementComponent>(true);
            blobHandler = state.GetComponentTypeHandle<MovementComponentBlob>(true);
            transformHandler = state.GetComponentTypeHandle<LocalTransform>(false);
            state.RequireForUpdate(_query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            componentHandler.Update(ref state);
            blobHandler.Update(ref state);
            transformHandler.Update(ref state);
            
            state.Dependency = new movementJob()
            {
                componentHandler = componentHandler,
                blobHandler = blobHandler,
                deltaTime = SystemAPI.Time.DeltaTime,
                transformHandler = transformHandler
            }.ScheduleParallel(_query, state.Dependency);
            
            /*foreach (var (aiRO, transformRW) in SystemAPI.Query<RefRO<MovementComponent>, RefRW<LocalTransform>>())
            {
                ref readonly MovementComponent ai = ref aiRO.ValueRO;
                float3 dir = ai.Target - transformRW.ValueRO.Position;
                float dist = math.sqrt(dir.x * dir.x + dir.y * dir.y + dir.z * dir.z);
                if (dist <= 0.5) continue;
                float time = SystemAPI.Time.DeltaTime;
                ref LocalTransform transform = ref transformRW.ValueRW;
                transform.Position += (dir / dist) * (ai.Speed * time);
                transform.Rotation = TransformHelpers.LookAtRotation(transform.Position, ai.Target, transform.Up());
            }*/
        }

        [BurstCompile]
        private struct movementJob : IJobChunk
        {
            [ReadOnly] public ComponentTypeHandle<MovementComponent> componentHandler;
            [ReadOnly] public ComponentTypeHandle<MovementComponentBlob> blobHandler;
            public ComponentTypeHandle<LocalTransform> transformHandler;
            public float deltaTime;
            
            [BurstCompile]
            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                NativeArray<MovementComponent> movementComponents = chunk.GetNativeArray(ref componentHandler);
                NativeArray<MovementComponentBlob> blobs = chunk.GetNativeArray(ref blobHandler);
                NativeArray<LocalTransform> localTransforms = chunk.GetNativeArray(ref transformHandler);
                for (int i = 0; i < chunk.Count; i++)
                {
                    MovementComponent currentMovementComponent = movementComponents[i];
                    LocalTransform currentLocalTransform = localTransforms[i];

                    float3 dir;
                    /*if (!currentMovementComponent.Target.Equals(null))
                    {
                        float3 position = currentMovementComponent.Target.Position;
                        currentLocalTransform.Rotation = TransformHelpers.LookAtRotation(
                            currentLocalTransform.Position, position, currentLocalTransform.Up());
                        dir = position - currentLocalTransform.Position;
                    }
                    else
                    {
                        dir = currentMovementComponent.Direction;
                    }*/

                    dir = currentMovementComponent.Direction;
                    float dist = math.sqrt(dir.x * dir.x + dir.y * dir.y + dir.z * dir.z);
                    if (dist <= 0.5) continue;
                    currentLocalTransform.Position += (dir / dist) * (blobs[i].config.Value.Speed * deltaTime)+ (float3)Vector3.up * (currentMovementComponent.Gravity * deltaTime);
                    
                    localTransforms[i] = currentLocalTransform;
                }
            }
        }
    }
}
