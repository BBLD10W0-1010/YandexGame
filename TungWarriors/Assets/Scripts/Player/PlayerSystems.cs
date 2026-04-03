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
        var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(systemState.WorldUnmanaged);
        var physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        foreach (var (transform, entity) in
                 SystemAPI.Query<RefRO<LocalTransform>>().WithAll<PlayerTag>().WithEntityAccess())
        {
            var weapons = SystemAPI.GetBuffer<PlayerWeaponData>(entity);

            int damageBonus = 0;
            if (SystemAPI.HasComponent<PlayerDamageBonus>(entity))
            {
                damageBonus = SystemAPI.GetComponent<PlayerDamageBonus>(entity).Value;
            }

            var playerPosition = transform.ValueRO.Position;

            for (int weaponIndex = 0; weaponIndex < weapons.Length; weaponIndex++)
            {
                var weapon = weapons[weaponIndex];

                if (weapon.NextFireTime > elapsedTime)
                    continue;

                var minDetectPosition = playerPosition - weapon.DetectionSize;
                var maxDetectPosition = playerPosition + weapon.DetectionSize;

                var aabbInput = new OverlapAabbInput
                {
                    Aabb = new Aabb
                    {
                        Min = minDetectPosition,
                        Max = maxDetectPosition
                    },
                    Filter = weapon.CollisionFilter
                };

                var overlapHits = new NativeList<int>(systemState.WorldUpdateAllocator);
                if (!physicsWorldSingleton.OverlapAabb(aabbInput, ref overlapHits))
                    continue;

                float minDistanceSq = float.MaxValue;
                float3 closestEnemyPosition = float3.zero;

                foreach (var overlapHit in overlapHits)
                {
                    var enemyPosition = physicsWorldSingleton.Bodies[overlapHit].WorldFromBody.pos;
                    var distanceSq = math.distancesq(playerPosition.xy, enemyPosition.xy);

                    if (distanceSq < minDistanceSq)
                    {
                        minDistanceSq = distanceSq;
                        closestEnemyPosition = enemyPosition;
                    }
                }

                var toEnemy = closestEnemyPosition - playerPosition;
                var baseAngle = math.atan2(toEnemy.y, toEnemy.x);
                var baseRotation = quaternion.RotateZ(baseAngle);

                int pelletCount = math.max(1, weapon.PelletCount);
                float totalSpreadRad = math.radians(weapon.SpreadAngle);

                PlasmaBlastData projectileData = default;
                bool hasDamageBonus = false;

                if (damageBonus > 0)
                {
                    projectileData = SystemAPI.GetComponent<PlasmaBlastData>(weapon.AttackPrefab);
                    projectileData.AttackDamage += damageBonus;
                    hasDamageBonus = true;
                }

                for (int i = 0; i < pelletCount; i++)
                {
                    float t = pelletCount == 1 ? 0.5f : (float)i / (pelletCount - 1);
                    float angleOffset = math.lerp(-totalSpreadRad * 0.5f, totalSpreadRad * 0.5f, t);

                    var pelletRotation = math.mul(baseRotation, quaternion.RotateZ(angleOffset));
                    float3 pelletForward = math.mul(pelletRotation, new float3(1f, 0f, 0f));
                    float3 pelletSpawnPosition = playerPosition + pelletForward * weapon.AttackSpawnOffset;

                    var newAttack = ecb.Instantiate(weapon.AttackPrefab);
                    ecb.SetComponent(newAttack, LocalTransform.FromPositionRotation(pelletSpawnPosition, pelletRotation));

                    if (hasDamageBonus)
                    {
                        ecb.SetComponent(newAttack, projectileData);
                    }
                }

                weapon.NextFireTime = elapsedTime + weapon.CooldownTime;
                weapons[weaponIndex] = weapon;
            }
        }
    }
}