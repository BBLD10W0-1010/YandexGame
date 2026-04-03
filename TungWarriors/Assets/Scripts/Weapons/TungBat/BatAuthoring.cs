using Unity.Entities;
using UnityEngine;

public class BatWeaponAuthoring : MonoBehaviour
{
    public int Damage;
    public float Cooldown;
    public float Range;
    public float ConeAngleDegrees;

    private class Baker : Baker<BatWeaponAuthoring>
    {
        public override void Bake(BatWeaponAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new BatWeaponData
            {
                Damage = authoring.Damage,
                Cooldown = authoring.Cooldown,
                Range = authoring.Range,
                ConeAngleDegrees = authoring.ConeAngleDegrees
            });

            AddComponent(entity, new BatWeaponCooldown
            {
                NextAttackTime = 0d
            });
        }
    }
}