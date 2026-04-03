using Unity.Entities;

public struct ShotgunData : IComponentData
{
    public Entity PlasmaBlastPrefab;

    public float Cooldown;
    public float CooldownTimer;

    public int PelletCount;
    public float SpreadAngle;
    public float SpawnOffset;

    public byte AutoFire;
}