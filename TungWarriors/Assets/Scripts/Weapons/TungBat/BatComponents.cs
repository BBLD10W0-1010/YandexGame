using Unity.Entities;

public struct BatWeaponData : IComponentData
{
    public int Damage;
    public float Cooldown;
    public float Range;
    public float ConeAngleDegrees;
}

public struct BatWeaponCooldown : IComponentData
{
    public double NextAttackTime;
}