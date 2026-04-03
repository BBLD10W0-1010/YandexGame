using Unity.Entities;
using UnityEngine;

public class CardDamageBonusAuthoring : MonoBehaviour
{
    public int Value;
    private class Baker : Baker<CardDamageBonusAuthoring>
    {
        public override void Bake(CardDamageBonusAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.None), new CardDamageBonusEffect { Value = authoring.Value });
        }
    }
}