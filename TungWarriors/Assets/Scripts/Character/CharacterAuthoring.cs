using Assets.Scripts.DeathConsequencesSystems;
using Unity.Entities;
using UnityEngine;

public class CharacterAuthoring : MonoBehaviour
{
    public float MoveSpeed;
    public int HitPoints;
    private class Baker : Baker<CharacterAuthoring>
    {
        public override void Bake(CharacterAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<CharacterMoveDirection>(entity);
            AddComponent<InitializeCharacterFlag>(entity);
            AddComponent(entity, new CharacterMoveSpeed()
            {
                Value = authoring.MoveSpeed
            });
            AddComponent(entity, new CharacterMaxHitPoints { Value = authoring.HitPoints });
            AddComponent(entity, new CharacterCurrentHitPoints { Value = authoring.HitPoints });
            AddBuffer<DamageThisFrame>(entity);
            AddComponent<DestroyEntityFlag>(entity);
            SetComponentEnabled<DestroyEntityFlag>(entity, false);
            AddComponent<DeathEntityFlag>(entity);
            SetComponentEnabled<DeathEntityFlag>(entity, false);
        }
    }
}
