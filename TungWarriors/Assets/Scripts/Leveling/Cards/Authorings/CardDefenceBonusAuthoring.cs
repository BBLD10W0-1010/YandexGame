using Unity.Entities;
using UnityEngine;

public class CardDefenseBonusAuthoring : MonoBehaviour
{
    public int Value;
    private class Baker : Baker<CardDefenseBonusAuthoring>
    {
        public override void Bake(CardDefenseBonusAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.None), new CardDefenseBonusEffect { Value = authoring.Value });
        }
    }
}