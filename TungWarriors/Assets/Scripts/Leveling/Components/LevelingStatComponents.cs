using Unity.Entities;
using Unity.Mathematics;

public struct CharacterMoveSpeedBonus : IComponentData
{
    public float Value;
}

public struct PlayerDamageBonus : IComponentData
{
    public int Value;
}

public struct CharacterDefense : IComponentData
{
    public int Value;
}

public struct CharacterHealthRegen : IComponentData
{
    public float ValuePerSecond;
    public float Accumulator;
}

public struct LastNonZeroMoveDirection : IComponentData
{
    public float2 Value;
}