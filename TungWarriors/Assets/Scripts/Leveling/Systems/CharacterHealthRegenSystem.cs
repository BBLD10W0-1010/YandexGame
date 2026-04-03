using Assets.Scripts.DeathConsequencesSystems;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ProcessDamageThisFrameSystem))]
public partial struct CharacterHealthRegenSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (regen, currentHp, maxHp, entity) in
                 SystemAPI.Query<RefRW<CharacterHealthRegen>, RefRW<CharacterCurrentHitPoints>, CharacterMaxHitPoints>()
                     .WithAll<PlayerTag>()
                     .WithEntityAccess())
        {
            if (regen.ValueRO.ValuePerSecond <= 0f)
                continue;
            if (currentHp.ValueRO.Value >= maxHp.Value)
                continue;
            if (SystemAPI.HasComponent<DeathEntityFlag>(entity) &&
                SystemAPI.IsComponentEnabled<DeathEntityFlag>(entity))
                continue;
            regen.ValueRW.Accumulator += regen.ValueRO.ValuePerSecond * deltaTime;
            var healAmount = (int)math.floor(regen.ValueRO.Accumulator);
            if (healAmount <= 0) 
                continue;
            regen.ValueRW.Accumulator -= healAmount;
            currentHp.ValueRW.Value = math.min(currentHp.ValueRO.Value + healAmount, maxHp.Value);
        }
    }
}