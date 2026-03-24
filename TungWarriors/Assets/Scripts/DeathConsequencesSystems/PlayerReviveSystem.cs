using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace Assets.Scripts.DeathConsequencesSystems
{
    [UpdateInGroup(typeof(DeathConsequencesGroup))]
    public partial struct PlayerReviveSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (revivesCount, currentHealth, maxHealth, entity) in
                     SystemAPI.Query<RefRW<RevivePlayerCount>, RefRW<CharacterCurrentHitPoints>, RefRW<CharacterMaxHitPoints>>()
                     .WithAll<DestroyEntityFlag>()
                     .WithAll<PlayerTag>()
                     .WithEntityAccess())
            {
                if (revivesCount.ValueRW.Value > 0)
                {
                    revivesCount.ValueRW.Value--;
                    currentHealth.ValueRW.Value = maxHealth.ValueRW.Value;
                    SystemAPI.SetComponentEnabled<DestroyEntityFlag>(entity, false);
                }
                else if (!revivesCount.ValueRW.IsAdvUsed)
                {
                    SystemAPI.SetComponentEnabled<DestroyEntityFlag>(entity, false);
                    GameUIController.Instance.SwitchDeathPanel();
                }
            }
        }
    }
}
