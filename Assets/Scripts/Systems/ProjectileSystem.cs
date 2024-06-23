
using System.ComponentModel;
using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct ProjectileSystem : ISystem
{
    [ReadOnly(true)] ComponentLookup<LocalTransform> positionLookup;
    ComponentLookup<ProjectileComponentBlob> impactLookup;
    BufferLookup<HitList> hitListLookup;
    ComponentLookup<HealthComponent> healthLookup;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        positionLookup = SystemAPI.GetComponentLookup<LocalTransform>();
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
            ecb = ecb
        }.Schedule(simulation, state.Dependency);
    }

    [BurstCompile]
    private struct ProjectileHitJob : ITriggerEventsJob
    {
        [ReadOnly(true)] public ComponentLookup<LocalTransform> Positions;
        public ComponentLookup<ProjectileComponentBlob> Projectiles;
        public ComponentLookup<HealthComponent> EnemiesHealth;
        public EntityCommandBuffer ecb;
        public BufferLookup<HitList> HitLists;
        
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
            
            HealthComponent hp = EnemiesHealth[enemy];
            hp.Health -= 5;
            EnemiesHealth[enemy] = hp;
            
            if (hp.Health <= 0)
                ecb.DestroyEntity(enemy);

            var blob = Projectiles[projectile].Blob.Value;
            
            Entity impactEntity = ecb.Instantiate(Projectiles[projectile].OnHit);
            ecb.SetComponent(impactEntity, LocalTransform.FromPosition(Positions[enemy].Position));
            
            if (blob.MaxImpactCount <= HitLists[projectile].Length)
                ecb.DestroyEntity(projectile);
        }
    }
}


