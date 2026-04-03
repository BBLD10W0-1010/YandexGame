using Unity.Entities;
using UnityEngine;

public readonly struct LevelUpCardViewData
{
    public readonly Entity CardEntity;
    public readonly string Title;
    public readonly string Description;
    public readonly Sprite Icon;

    public LevelUpCardViewData(Entity cardEntity, string title, string description, Sprite icon)
    {
        CardEntity = cardEntity;
        Title = title;
        Description = description;
        Icon = icon;
    }
}