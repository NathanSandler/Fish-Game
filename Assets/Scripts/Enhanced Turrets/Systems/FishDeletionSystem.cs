using Components;
using Enhanced_Turrets.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Enhanced_Turrets.Systems
{
    [BurstCompile]
    public partial struct FishDeletionSystem : ISystem
    {
        private EntityQuery _query;

        public void OnCreate(ref SystemState state)
        {
            _query = state.GetEntityQuery(ComponentType.ReadOnly<FishComponent>());
        }
        
        public void OnDestroy(ref SystemState state)
        {
            var entities = _query.ToEntityArray(Allocator.TempJob);
            foreach (var Turret in SystemAPI.Query<DynamicBuffer<TargetableComponent>>())
            {
                for (int i = Turret.Length - 1; i >= 0; i--)
                {
                    Entity e = Turret[i].entity;
                    int index = -1;
                    foreach (var entity in entities)
                    {
                        if (e == entity)
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index != -1)
                    {
                        Turret.RemoveAt(index);
                    }
                }
            }
            UnityEngine.Debug.Log($"Entity is being destroyed. {entities.Length}");
            
            entities.Dispose();
        }
    }
}