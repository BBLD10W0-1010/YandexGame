using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Physics;
using Unity.Collections;


public partial class PlayerInputSystem : SystemBase
{
    private SurvivorsInput _inputActions;

    protected override void OnCreate()
    {
        _inputActions = new SurvivorsInput();
        _inputActions.Enable();
    }
    protected override void OnUpdate()
    {
        var currentInput = (float2)_inputActions.Player.Move.ReadValue<Vector2>();
        foreach (var direction in SystemAPI.Query<RefRW<CharacterMoveDirection>>().WithAll<PlayerTag>())
        {
            direction.ValueRW.Value = currentInput;
        }
        if (math.lengthsq(currentInput) > 0.0001f)
        {
            var normalized = math.normalize(currentInput);
            foreach (var lastDir in SystemAPI.Query<RefRW<LastNonZeroMoveDirection>>().WithAll<PlayerTag>())
                lastDir.ValueRW.Value = normalized;
        }
    }
}

public partial struct PlayerAttackSystem : ISystem
{
    public void OnCreate(ref SystemState systemState)
    {
        systemState.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
    }
    public void OnUpdate(ref SystemState systemState)
    {
        var elapsedTime = SystemAPI.Time.ElapsedTime;
        var entityCommandBufferSystem = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = entityCommandBufferSystem.CreateCommandBuffer(systemState.WorldUnmanaged);
        var physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        foreach (var (expirationTimestamp, attackData, transform, entity) in SystemAPI.Query<RefRW<PlayerCooldownExpirationTimestamp>, PlayerAttackData, LocalTransform>()
            .WithEntityAccess())
        {
            if (expirationTimestamp.ValueRO.Value > elapsedTime) continue;
            var spawnPosition = transform.Position;
            var minDetectPosition = spawnPosition - attackData.DetectionSize;
            var maxDetectPosition = spawnPosition + attackData.DetectionSize;
            var aabbinput = new OverlapAabbInput
            {
                Aabb = new Aabb
                {
                    Min = minDetectPosition,
                    Max = maxDetectPosition
                },
                Filter = attackData.CollisionFilter
            };

            var overlapHits = new NativeList<int>(systemState.WorldUpdateAllocator);
            if (!physicsWorldSingleton.OverlapAabb(aabbinput, ref overlapHits)) continue;

            var maxDistanceSquared = float.MaxValue;
            var closestEnemyPosition = float3.zero;
            foreach (var overlapHit in overlapHits)
            {
                var currentEnemyPosition = physicsWorldSingleton.Bodies[overlapHit].WorldFromBody.pos;
                var distanceToPlayerSquared = math.distancesq(spawnPosition.xy, currentEnemyPosition.xy);
                if (distanceToPlayerSquared < maxDistanceSquared)
                {
                    maxDistanceSquared = distanceToPlayerSquared;
                    closestEnemyPosition = currentEnemyPosition;
                }
            }

            var vectorToClosestEnemy = closestEnemyPosition - spawnPosition;
            var angleToClosestEnemy = math.atan2(vectorToClosestEnemy.y, vectorToClosestEnemy.x);
            var spawnOrientation = quaternion.Euler(0f, 0f, angleToClosestEnemy);
            var newAttack = ecb.Instantiate(attackData.AttackPrefab);
            ecb.SetComponent(newAttack, LocalTransform.FromPositionRotation(spawnPosition, spawnOrientation));
            expirationTimestamp.ValueRW.Value = elapsedTime + attackData.CooldownTime;
            if (SystemAPI.HasComponent<PlayerDamageBonus>(entity))
            {
                var bonus = SystemAPI.GetComponent<PlayerDamageBonus>(entity).Value;
                if (bonus > 0)
                {
                    var projectileData = SystemAPI.GetComponent<PlasmaBlastData>(attackData.AttackPrefab);
                    projectileData.AttackDamage += bonus;
                    ecb.SetComponent(newAttack, projectileData);
                }
            }
            expirationTimestamp.ValueRW.Value = elapsedTime + attackData.CooldownTime;
        }
    }
}