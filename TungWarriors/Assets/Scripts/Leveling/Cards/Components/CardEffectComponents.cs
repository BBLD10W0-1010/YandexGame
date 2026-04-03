using Unity.Entities;

public struct CardDamageBonusEffect : IComponentData
{
    public int Value;
}

public struct CardDefenseBonusEffect : IComponentData
{
    public int Value;
}

public struct CardHealthRegenEffect : IComponentData
{
    public float ValuePerSecond;
}

public struct CardMoveSpeedBonusEffect : IComponentData
{
    public float Value;
}

public struct CardUnlockBatWeaponEffect : IComponentData
{
    public int Damage;
    public float Cooldown;
    public float Range;
    public float ConeAngleDegrees;
}