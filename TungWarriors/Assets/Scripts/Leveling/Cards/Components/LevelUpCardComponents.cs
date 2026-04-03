using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct LevelUpCardMeta : IComponentData
{
    public FixedString64Bytes Title;
    public FixedString128Bytes Description;
    public UnityObjectRef<Sprite> Icon;
}

public struct LevelUpCardPoolTag : IComponentData { }

public struct InitialLevelUpCardElement : IBufferElementData
{
    public Entity Value;
}