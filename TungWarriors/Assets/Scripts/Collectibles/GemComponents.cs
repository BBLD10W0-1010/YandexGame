using Unity.Entities;

public struct GemTag : IComponentData { }

public struct GemsCollectedCount : IComponentData
{
    public int Value;
}