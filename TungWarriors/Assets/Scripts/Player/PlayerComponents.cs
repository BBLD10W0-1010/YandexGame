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
}

public struct PlayerCooldownExpirationTimestamp : IComponentData
{
    public double Value;
}