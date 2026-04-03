using Assets.Scripts.DeathConsequencesSystems;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct CharacterInitializationSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (mass, shouldInitialize) in SystemAPI.Query<RefRW<PhysicsMass>, EnabledRefRW<InitializeCharacterFlag>>())
        {
            mass.ValueRW.InverseInertia = float3.zero;
            shouldInitialize.ValueRW = false;
        }
    }
}
public partial struct CharacterMoveSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (velocity, direction, speed, entity) in SystemAPI.Query<RefRW<PhysicsVelocity>, CharacterMoveDirection, CharacterMoveSpeed>()
            .WithEntityAccess())
        {
            var totalSpeed = speed.Value;
            if (SystemAPI.HasComponent<CharacterMoveSpeedBonus>(entity))
                totalSpeed += SystemAPI.GetComponent<CharacterMoveSpeedBonus>(entity).Value;
            var moveStep2d = direction.Value * totalSpeed;
            velocity.ValueRW.Linear = new float3(moveStep2d, 0f);
        }
    }
}

public partial struct ProcessDamageThisFrameSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (hitpoints, damageThisFrame, entity) in SystemAPI.Query<RefRW<CharacterCurrentHitPoints>, DynamicBuffer<DamageThisFrame>>().WithPresent<DeathEntityFlag>().WithEntityAccess())
        {

            if (damageThisFrame.IsEmpty) continue;
            var defense = 0;
            if (SystemAPI.HasComponent<CharacterDefense>(entity))
                defense = SystemAPI.GetComponent<CharacterDefense>(entity).Value;
            foreach (var damage in damageThisFrame)
            {
                hitpoints.ValueRW.Value -= math.max(0, damage.Value - defense);
            }
            damageThisFrame.Clear();
            if (hitpoints.ValueRO.Value <= 0)
            {
                SystemAPI.SetComponentEnabled<DeathEntityFlag>(entity, true);
            }
        }
    }
}