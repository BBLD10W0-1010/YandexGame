using Unity.Entities;

public struct PlasmaBlastData : IComponentData
{
    public float MoveSpeed;
    public int AttackDamage;
    public float PlayerDamageCoefficient;
    public float PlayerMoveSpeedCoefficient;
    public float CritChanceCoefficient;
    public float CritDamageCoefficient;
}
