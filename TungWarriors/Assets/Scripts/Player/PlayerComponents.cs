using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public struct RevivePlayerCount : IComponentData
{
    public int Value;
    public bool IsAdvUsed;
}

public struct PlayerTag : IComponentData { }

public struct PlayerAttackData : IComponentData
{
    public Entity AttackPrefab;
    public float CooldownTime;
    public float3 DetectionSize;
    public CollisionFilter CollisionFilter;

    public int PelletCount;
    public float SpreadAngle;
    public float AttackSpawnOffset;
}

public struct PlayerWeaponData : IBufferElementData
{
    public Entity AttackPrefab;
    public float CooldownTime;
    public double NextFireTime;

    public float3 DetectionSize;
    public CollisionFilter CollisionFilter;

    public int PelletCount;
    public float SpreadAngle;
    public float AttackSpawnOffset;
}
public struct PlayerCooldownExpirationTimestamp : IComponentData
{
    public double Value;
}