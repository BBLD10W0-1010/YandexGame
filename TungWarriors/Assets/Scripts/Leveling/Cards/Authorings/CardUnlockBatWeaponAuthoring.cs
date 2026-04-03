using Unity.Entities;
using UnityEngine;

public class CardUnlockBatWeaponAuthoring : MonoBehaviour
{
    public GameObject BatPrefab;

    private class Baker : Baker<CardUnlockBatWeaponAuthoring>
    {
        public override void Bake(CardUnlockBatWeaponAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.None), new CardUnlockBatWeaponEffect
            {
                BatPrefab = GetEntity(authoring.BatPrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}