using Unity.Entities;
using UnityEngine;

public class LevelUpCardAuthoring : MonoBehaviour
{
    public string Title;
    [TextArea] public string Description;
    public Sprite Icon;

    private class Baker : Baker<LevelUpCardAuthoring>
    {
        public override void Bake(LevelUpCardAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new LevelUpCardMeta
            {
                Title = authoring.Title,
                Description = authoring.Description,
                Icon = authoring.Icon
            });
        }
    }
}