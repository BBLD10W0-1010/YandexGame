using Assets.Scripts.DeathConsequencesSystems;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Physics;

public class PlayerAuthoring : MonoBehaviour
{
    [System.Serializable]
    public struct WeaponAuthoringData
    {
        public GameObject AttackPrefab;
        public float CooldownTime;
        public float DetectionSize;
        public int PelletCount;
        public float SpreadAngle;
        public float AttackSpawnOffset;
    }

    [Header("Pistol")]
    public WeaponAuthoringData Pistol;

    [Header("Shotgun")]
    public WeaponAuthoringData Shotgun;

    public GameObject WorldUiPrefab;

    private class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<PlayerTag>(entity);
            AddComponent<InitializeCameraTargetTag>(entity);
            AddComponent<CameraTarget>(entity);

            var enemyLayer = LayerMask.NameToLayer("Enemy");
            var enemyLayerMask = (uint)math.pow(2, enemyLayer);

            var attackCollisionFilter = new CollisionFilter
            {
                BelongsTo = uint.MaxValue,
                CollidesWith = enemyLayerMask
            };

            AddComponent(entity, new RevivePlayerCount
            {
                Value = 1,
                IsAdvUsed = false
            });

            AddComponent<PlayerThinkingFlag>(entity);
            SetComponentEnabled<PlayerThinkingFlag>(entity, false);

            AddComponent(entity, new GemsCollectedCount { Value = 0 });
            AddComponent<UpdateGemUIFlag>(entity);

            AddComponent(entity, new PlayerWorldUIPrefab
            {
                Value = authoring.WorldUiPrefab
            });

            var weapons = AddBuffer<PlayerWeaponData>(entity);

            weapons.Add(new PlayerWeaponData
            {
                AttackPrefab = GetEntity(authoring.Pistol.AttackPrefab, TransformUsageFlags.Dynamic),
                CooldownTime = authoring.Pistol.CooldownTime,
                NextFireTime = 0,
                DetectionSize = new float3(authoring.Pistol.DetectionSize),
                CollisionFilter = attackCollisionFilter,
                PelletCount = authoring.Pistol.PelletCount,
                SpreadAngle = authoring.Pistol.SpreadAngle,
                AttackSpawnOffset = authoring.Pistol.AttackSpawnOffset
            });

            weapons.Add(new PlayerWeaponData
            {
                AttackPrefab = GetEntity(authoring.Shotgun.AttackPrefab, TransformUsageFlags.Dynamic),
                CooldownTime = authoring.Shotgun.CooldownTime,
                NextFireTime = 0,
                DetectionSize = new float3(authoring.Shotgun.DetectionSize),
                CollisionFilter = attackCollisionFilter,
                PelletCount = authoring.Shotgun.PelletCount,
                SpreadAngle = authoring.Shotgun.SpreadAngle,
                AttackSpawnOffset = authoring.Shotgun.AttackSpawnOffset
            });
        }
    }
}