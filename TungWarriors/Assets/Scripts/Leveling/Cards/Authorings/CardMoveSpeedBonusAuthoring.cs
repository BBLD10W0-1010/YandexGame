using Unity.Entities;
using UnityEngine;

public class CardMoveSpeedBonusAuthoring : MonoBehaviour
{
    public float Value;
    private class Baker : Baker<CardMoveSpeedBonusAuthoring>
    {
        public override void Bake(CardMoveSpeedBonusAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.None), new CardMoveSpeedBonusEffect { Value = authoring.Value });
        }
    }
}