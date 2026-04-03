using Unity.Entities;
using UnityEngine;

public class CardUnlockBatWeaponAuthoring : MonoBehaviour
{
    public int Damage;
    public float Cooldown;
    public float Range;
    public float ConeAngleDegrees;

    private class Baker : Baker<CardUnlockBatWeaponAuthoring>
    {
        public override void Bake(CardUnlockBatWeaponAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.None), new CardUnlockBatWeaponEffect
            {
                Damage = authoring.Damage,
                Cooldown = authoring.Cooldown,
                Range = authoring.Range,
                ConeAngleDegrees = authoring.ConeAngleDegrees
            });
        }
    }
}