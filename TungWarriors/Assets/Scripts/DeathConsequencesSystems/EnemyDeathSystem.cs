using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.DeathConsequencesSystems
{
    [UpdateInGroup(typeof(DeathConsequencesGroup), OrderLast = true)]
    public partial struct EnemyDeathSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EnemySpawnState>();
        }
        public void OnUpdate(ref SystemState state)
        {
            var spawnState = SystemAPI.GetSingletonRW<EnemySpawnState>();

            foreach (var (_, entity) in SystemAPI.Query<DeathEntityFlag>().WithAll<EnemyTag>().WithAll<DeathEntityFlag>().WithEntityAccess())
            {
                Debug.Log("Enemy death");
                SystemAPI.SetComponentEnabled<DestroyEntityFlag>(entity, true);
                spawnState.ValueRW.CurrentSpawnedEnemies--;
            }
        }
    }
}
