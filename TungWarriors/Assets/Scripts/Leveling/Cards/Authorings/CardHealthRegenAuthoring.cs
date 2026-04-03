using Unity.Entities;
using UnityEngine;

public class CardHealthRegenAuthoring : MonoBehaviour
{
    public float ValuePerSecond;
    private class Baker : Baker<CardHealthRegenAuthoring>
    {
        public override void Bake(CardHealthRegenAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.None), new CardHealthRegenEffect { ValuePerSecond = authoring.ValuePerSecond });
        }
    }
}