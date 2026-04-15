using Unity.Entities;
using UnityEngine;

public partial class BatAuthoring : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public float speed = 1.0f;
    public float damage = 1.0f;
    public float CooldownTime = 1.0f;
    private class Baker : Baker<BatAuthoring>
    {
        public override void Bake(BatAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BatAttackData
            {
               HitPoints = authoring.damage,
               CooldownTime = authoring.CooldownTime,
               Speed = authoring.speed,
               attackPrefab = GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic)
            });

        }
    }
}
