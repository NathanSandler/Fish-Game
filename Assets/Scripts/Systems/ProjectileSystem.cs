
using System.ComponentModel;
using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct ProjectileSystem : ISystem
{
    [ReadOnly(true)] ComponentLookup<LocalToWorld> positionLookup;
    ComponentLookup<ProjectileComponentBlob> impactLookup;
    BufferLookup<HitList> hitListLookup;
    ComponentLookup<HealthComponent> healthLookup;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        positionLookup = SystemAPI.GetComponentLookup<LocalToWorld>();
        impactLookup = SystemAPI.GetComponentLookup<ProjectileComponentBlob>();
        hitListLookup = SystemAPI.GetBufferLookup<HitList>();
        healthLookup = SystemAPI.GetComponentLookup<HealthComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().
            CreateCommandBuffer(state.WorldUnmanaged);
        SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();

        positionLookup.Update(ref state);
        healthLookup.Update(ref state);
        impactLookup.Update(ref state);
        hitListLookup.Update(ref state);

        state.Dependency = new ProjectileHitJob()
        {
            Projectiles = impactLookup,
            EnemiesHealth = healthLookup,
            Positions = positionLookup,
            HitLists = hitListLookup,
            ECB = ecb
        }.Schedule(simulation, state.Dependency);
    }

    [BurstCompile]
    private struct ProjectileHitJob : ITriggerEventsJob
    {
        [ReadOnly(true)] public ComponentLookup<LocalToWorld> Positions;
        public ComponentLookup<ProjectileComponentBlob> Projectiles;
        public ComponentLookup<HealthComponent> EnemiesHealth;
        public EntityCommandBuffer ECB;
        public BufferLookup<HitList> HitLists;
        private static readonly float3 Up = new float3(0, 1, 0);

        [BurstCompile]
        public void Execute(TriggerEvent triggerEvent)
        {
            Entity projectile = Entity.Null;
            Entity enemy = Entity.Null;
            
            if (Projectiles.HasComponent(triggerEvent.EntityA))
                projectile = triggerEvent.EntityA;
            if (Projectiles.HasComponent(triggerEvent.EntityB))
                projectile = triggerEvent.EntityB;
            if (EnemiesHealth.HasComponent(triggerEvent.EntityA))
                enemy = triggerEvent.EntityA;
            if (EnemiesHealth.HasComponent(triggerEvent.EntityB))
                enemy = triggerEvent.EntityB;
            
            if (Entity.Null.Equals(projectile) 
                || Entity.Null.Equals(enemy)) return;
            
            DynamicBuffer<HitList> hits = HitLists[projectile];
            for (int i = 0; i < hits.Length; i++)
            {
                if(hits[i].entity.Equals(enemy))
                    return;
            }
            
            hits.Add(new HitList { entity = enemy });
            var blob = Projectiles[projectile].Blob.Value;

            HealthComponent hp = EnemiesHealth[enemy];
            hp.Health -= blob.Damage;
            EnemiesHealth[enemy] = hp;
            
            if (hp.Health <= 0)
                ECB.DestroyEntity(enemy);

            Entity impactEntity = ECB.Instantiate(Projectiles[projectile].OnHit);
            ECB.SetComponent(impactEntity, LocalTransform.FromPositionRotation(Positions[enemy].Position, quaternion.LookRotation(-Positions[projectile].Forward, Up)));
            
            if (blob.MaxImpactCount <= HitLists[projectile].Length)
                ECB.DestroyEntity(projectile);
        }
    }
}


