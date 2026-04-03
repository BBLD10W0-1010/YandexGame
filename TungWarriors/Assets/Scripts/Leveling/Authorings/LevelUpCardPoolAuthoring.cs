using Unity.Entities;
using UnityEngine;

public class LevelUpCardPoolAuthoring : MonoBehaviour
{
    public GameObject[] InitialCards;

    private class Baker : Baker<LevelUpCardPoolAuthoring>
    {
        public override void Bake(LevelUpCardPoolAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent<LevelUpCardPoolTag>(entity);
            var buffer = AddBuffer<InitialLevelUpCardElement>(entity);
            foreach (var card in authoring.InitialCards)
            {
                if (card == null) continue;
                buffer.Add(new InitialLevelUpCardElement
                {
                    Value = GetEntity(card, TransformUsageFlags.None)
                });
            }
        }
    }
}