using Unity.Entities;
using Unity.Mathematics;

public struct PlayerLevel : IComponentData
{
    public int Value;
}

public struct PlayerExperience : IComponentData
{
    public int Current;
    public int RequiredForNext;
}

public struct InitializePlayerLevelingFlag : IComponentData, IEnableableComponent { }

public struct ShowLevelUpSelectionFlag : IComponentData, IEnableableComponent { }

public struct SelectedLevelUpCard : IComponentData, IEnableableComponent
{
    public Entity Value;
}

public struct PlayerCardRandom : IComponentData
{
    public Random Value;
}

public struct AvailableLevelUpCardElement : IBufferElementData
{
    public Entity Value;
}

public struct OfferedLevelUpCardElement : IBufferElementData
{
    public Entity Value;
}