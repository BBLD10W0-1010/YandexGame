using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine.Rendering;
using Unity.Mathematics;
using Unity.Burst;

public struct EnemyTag : IComponentData { }
[RequireComponent(typeof(CharacterAuthoring))]
public partial class EnemyAuthoring : MonoBehaviour
{
   private class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring enemyAuthoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<EnemyTag>(entity);
        }

        
    }

    public partial struct EnemyMoveToPlayerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerTag>();
        }
        public void OnUpdate(ref SystemState state)
        {
            var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
            var playerPosition = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position.xy;
            var moveToPlayerJob = new EnemyMoveToPlayerJob
            {
                playerPosition = playerPosition
            };
            state.Dependency = moveToPlayerJob.ScheduleParallel(state.Dependency);
        }
    }

    [BurstCompile]
    [WithAll(typeof(EnemyTag))]
    public partial struct EnemyMoveToPlayerJob : IJobEntity
    {
        public float2 playerPosition;
        private void Execute(ref CharacterMoveDirection characterMoveDirection, in LocalTransform localTransform)
        {
            var vectorToPlayer = playerPosition - localTransform.Position.xy;
            characterMoveDirection.Value = math.normalize(vectorToPlayer);
        }
    }
}
