using Assets.Scripts.DeathConsequencesSystems;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using System.Timers;
using Unity.Physics;
using Unity.Collections;
using UnityEngine.UI;

public struct PlayerTag : IComponentData {}

public struct RevivePlayerCount : IComponentData 
{
    public int Value;
    public bool IsAdvUsed;
}

public struct CameraTarget: IComponentData
{
    public UnityObjectRef<Transform> CameraTransform; 
}

public struct InitializeCameraTargetTag : IComponentData { }

public struct PlayerAttackData : IComponentData
{
    public Entity AttackPrefab;
    public float CooldownTime;
    public float3 DetectionSize;
    public CollisionFilter CollisionFilter;
}

public struct PlayerCooldownExpirationTimestamp : IComponentData
{
    public double Value;
}

public struct GemsCollectedCount : IComponentData
{
    public int Value;
}

public struct UpdateGemUIFlag : IComponentData, IEnableableComponent { }


public struct PlayerWorldUI : ICleanupComponentData
{
    public UnityObjectRef<Transform> CanvasTransform;
    public UnityObjectRef<Slider> HealthBarSlider;
}

public struct PlayerWorldUIPrefab : IComponentData
{
    public UnityObjectRef<GameObject> Value;
}

public class PlayerAuthoring : MonoBehaviour
{
    public GameObject AttackPrefab;
    public float CooldownTime;
    public float DetectionSize;
    public GameObject WorldUiPrefab;
    private class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PlayerTag>(entity);
            AddComponent<InitializeCameraTargetTag>(entity);
            AddComponent<CameraTarget>(entity);

            var EnemyLayer = LayerMask.NameToLayer("Enemy");
            var EnemyLayerMask = (uint)math.pow(2, EnemyLayer);

            var attackCollisionFilter = new CollisionFilter
            {
                BelongsTo = uint.MaxValue,
                CollidesWith = EnemyLayerMask
            };

            AddComponent(entity, new RevivePlayerCount()
            {
                Value = 1,
                IsAdvUsed = false
            });
            AddComponent<PlayerThinkingFlag>(entity);
            SetComponentEnabled<PlayerThinkingFlag>(entity, false);
            AddComponent(entity, new PlayerAttackData()
            {
                AttackPrefab = GetEntity(authoring.AttackPrefab, TransformUsageFlags.Dynamic),
                CooldownTime = authoring.CooldownTime,
                DetectionSize = new float3(authoring.DetectionSize),
                CollisionFilter = attackCollisionFilter
            });
            AddComponent<PlayerCooldownExpirationTimestamp>(entity);
            AddComponent(entity, new GemsCollectedCount { Value = 0 });
            AddComponent<UpdateGemUIFlag>(entity);
            AddComponent(entity, new PlayerWorldUIPrefab()
            {
                Value = authoring.WorldUiPrefab
            });

        }
    }
}

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct CameraInitializationSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<InitializeCameraTargetTag>();
    }

    public void OnUpdate(ref SystemState state)
    {
        if (CameraTargetSingleton.Instance == null)
        {
            return;
        }
        var cameraTargetTransform = CameraTargetSingleton.Instance.transform;

        var entityCommandBuffer = new EntityCommandBuffer(state.WorldUpdateAllocator);

        foreach (var (cameraTarget,entity) in SystemAPI.Query<RefRW<CameraTarget>>().WithAll<InitializeCameraTargetTag, PlayerTag>().WithEntityAccess())
        {
            cameraTarget.ValueRW.CameraTransform = cameraTargetTransform;
            entityCommandBuffer.RemoveComponent<InitializeCameraTargetTag>(entity);
        }
        entityCommandBuffer.Playback(state.EntityManager);
    }
}

[UpdateAfter(typeof(TransformSystemGroup))]
public partial struct MoveCameraSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (entity,cameraTarget) in SystemAPI.Query<LocalToWorld, CameraTarget>().WithAll<PlayerTag>().WithNone<InitializeCameraTargetTag>())
        {
            cameraTarget.CameraTransform.Value.position = new Vector3(entity.Position.x, entity.Position.y, -10f);
        }
    }
}
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
        foreach(var direction in SystemAPI.Query<RefRW<CharacterMoveDirection>>().WithAll<PlayerTag>())
        {
            direction.ValueRW.Value = currentInput;
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
        foreach (var (expirationTimestamp, attackData, transform)  in SystemAPI.Query<RefRW<PlayerCooldownExpirationTimestamp>, PlayerAttackData, LocalTransform>()) 
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

            ecb.SetComponent(newAttack, LocalTransform.FromPositionRotation(spawnPosition,spawnOrientation));

            expirationTimestamp.ValueRW.Value = elapsedTime + attackData.CooldownTime;

        }
    }
}
public partial struct UpdateGemUISystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (gemCount, shouldUpdateUI) in SystemAPI.Query<GemsCollectedCount, EnabledRefRW<UpdateGemUIFlag>>())
        {
            GameUIController.Instance.UpdateGemsCollectedText(gemCount.Value);
            shouldUpdateUI.ValueRW = false;
        }
    }
}

public partial struct PlayerWorldUISystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        foreach (var (uiPrefab, entity) in SystemAPI.Query<PlayerWorldUIPrefab>().WithNone<PlayerWorldUI>().WithEntityAccess())
        {
            var newWorldUI = Object.Instantiate(uiPrefab.Value.Value);
            ecb.AddComponent(entity, new PlayerWorldUI
            {
                CanvasTransform = newWorldUI.transform,
                HealthBarSlider = newWorldUI.GetComponentInChildren<Slider>()
            });
        }


        foreach (var (transform, worldUI, currentHitPoints, maxHitPoints) in SystemAPI.Query<LocalToWorld, PlayerWorldUI, CharacterCurrentHitPoints, CharacterMaxHitPoints>())
        {
            worldUI.CanvasTransform.Value.position = transform.Position;
            var healthValue = (float)currentHitPoints.Value / maxHitPoints.Value;
            worldUI.HealthBarSlider.Value.value = healthValue;
        }

        foreach (var (worldUI, entity) in SystemAPI.Query<PlayerWorldUI>().WithNone<LocalToWorld>().WithEntityAccess()) 
        {
            if (worldUI.CanvasTransform.Value != null)
            {
                Object.Destroy(worldUI.CanvasTransform.Value.gameObject);
            }

            ecb.RemoveComponent<PlayerWorldUI>(entity);
        }
        ecb.Playback(state.EntityManager);
    }
}