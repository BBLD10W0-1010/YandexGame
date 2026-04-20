using Unity.Entities;

public struct EquipmentStats : IComponentData
{
    public float Damage;
    public float Speed;
    public float Health;
    public float CritChance;
    public float CritDamage;
}

public struct PlayerBaseStats : IComponentData
{
    public float MoveSpeed;
    public int MaxHitPoints;
}

public struct PlayerResolvedStats : IComponentData
{
    public float Damage;
    public float MoveSpeedBonus;
    public int Defense;
    public float HealthRegen;
    public float CritChance;
    public float CritDamage;
    public int MaxHitPoints;
}

public struct InitializePlayerStatsFlag : IComponentData, IEnableableComponent { }

public struct RevivePlayerCount : IComponentData
{
    public int Value;
    public bool IsAdvUsed;
}

public struct PlayerTag : IComponentData { }
