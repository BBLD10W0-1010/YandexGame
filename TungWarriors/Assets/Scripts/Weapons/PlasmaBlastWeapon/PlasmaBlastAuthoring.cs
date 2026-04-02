using UnityEngine;
using Unity.Entities;

public class PlasmaBlastAuthoring : MonoBehaviour
{
    public float MoveSpeed;
    public int AttackDamage;

    private class Baker : Baker<PlasmaBlastAuthoring>
    {
        public override void Bake(PlasmaBlastAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlasmaBlastData
            {
                MoveSpeed = authoring.MoveSpeed,
                AttackDamage = authoring.AttackDamage
            });
            AddComponent<DestroyEntityFlag>(entity);
            SetComponentEnabled<DestroyEntityFlag>(entity, false);
        }
    }
}
