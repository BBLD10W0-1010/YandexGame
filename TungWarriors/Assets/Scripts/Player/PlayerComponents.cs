using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public struct EquipmentStats : IComponentData
{
    public float Damage;
    public float Speed;
    public float Health;
    public float CritChance;
    public float CritDamage;
}

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