using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct TurretSystem : ISystem
{
    private EntityQuery _query;
    private ComponentTypeHandle<ShootingComponent> _turretHandle;
    private ComponentTypeHandle<ShootingComponentBlob> _blobHandle;
    private ComponentTypeHandle<LocalToWorld> _localToWorldHandle;

    private BeginSimulationEntityCommandBufferSystem _beginSimECB;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _query = SystemAPI.QueryBuilder()
            .WithAllRW<ShootingComponent>()
            .WithAll<ActiveTurretComponent, ShootingComponentBlob>()
            .Build();

        _turretHandle = state.GetComponentTypeHandle<ShootingComponent>(false);
        _blobHandle = state.GetComponentTypeHandle<ShootingComponentBlob>(true);
        _localToWorldHandle = state.GetComponentTypeHandle<LocalToWorld>(true);

        _beginSimECB = state.World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

        state.RequireForUpdate(_query);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _turretHandle.Update(ref state);
        _blobHandle.Update(ref state);
        _localToWorldHandle.Update(ref state);

        EntityCommandBuffer.ParallelWriter ecbParallel = _beginSimECB.CreateCommandBuffer().AsParallelWriter();

        var job = new TurretJob
        {
            TurretHandle = _turretHandle,
            BlobHandle = _blobHandle,
            LocalToWorldHandle = _localToWorldHandle,
            EntityManager = state.EntityManager,
            DeltaTime = SystemAPI.Time.DeltaTime,
            ECB = ecbParallel
        };

        state.Dependency = job.ScheduleParallel(_query, state.Dependency);

        // Add this to ensure that the command buffer system knows when the job is complete
        _beginSimECB.AddJobHandleForProducer(state.Dependency);
    }

    [BurstCompile]
    private struct TurretJob : IJobChunk
    {
        public ComponentTypeHandle<ShootingComponent> TurretHandle;
        [ReadOnly] public ComponentTypeHandle<ShootingComponentBlob> BlobHandle;
        [ReadOnly] public ComponentTypeHandle<LocalToWorld> LocalToWorldHandle;

        public EntityManager EntityManager;
        public float DeltaTime;

        public EntityCommandBuffer.ParallelWriter ECB;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var shootingComponents = chunk.GetNativeArray(TurretHandle);
            var blobs = chunk.GetNativeArray(BlobHandle);
            var localToWorlds = chunk.GetNativeArray(LocalToWorldHandle);

            for (int i = 0; i < chunk.Count; i++)
            {
                var shoot = shootingComponents[i];
                float currentTime = shoot.currentTime + DeltaTime;

                var g = blobs[i].config.Value;
                if (currentTime >= g.maxTime)
                {
                    currentTime = 0;

                    LocalToWorld firePoint = localToWorlds[i];
                    for (int j = 0; j < g.numProjectile; j++)
                    {
                        Entity e = EntityManager.Instantiate(shoot.projectile);

                        // Generate random angles within the specified accuracy range
                        float anglex = math.radians(UnityEngine.Random.Range(-g.accuracy, g.accuracy));
                        float angley = math.radians(UnityEngine.Random.Range(-g.accuracy, g.accuracy));

                        // Create rotation quaternions from the random angles
                        quaternion randomRotationX = quaternion.AxisAngle(math.up(), anglex);
                        quaternion randomRotationY = quaternion.AxisAngle(math.right(), angley);

                        // Combine the random rotations
                        quaternion combinedRotation = math.mul(randomRotationY, randomRotationX);

                        // Transform the forward vector by the combined random rotations
                        float3 randomDirection = math.mul(combinedRotation, math.forward());

                        // Apply the firePoint rotation to get the final direction
                        float3 direction = math.mul(firePoint.Rotation, randomDirection);
                        direction = math.normalize(direction);

                        // Set up the entity using the ECB
                        int entityInQueryIndex = firstEntityIndex + i;
                        SetupProjectile(entityInQueryIndex, e, firePoint.Position, quaternion.LookRotation(direction, firePoint.Up), direction);
                    }
                }

                shoot.currentTime = currentTime;
                shootingComponents[i] = shoot;
            }
        }

        private void SetupProjectile(int entityInQueryIndex, Entity e, float3 pos, quaternion rot, float3 forward)
        {
            ECB.SetComponent(entityInQueryIndex, e, new LocalTransform
            {
                Position = pos,
                Rotation = rot,
                Scale = 0.2f
            });

            MovementComponent x = EntityManager.GetComponentData<MovementComponent>(e);
            ECB.SetComponent(entityInQueryIndex, e, new MovementComponent
            {
                Direction = forward,
                Gravity = x.Gravity,
            });
        }
    }
}
