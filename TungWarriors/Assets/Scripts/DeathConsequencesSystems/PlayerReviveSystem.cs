
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.DeathConsequencesSystems
{
    /// <summary>
    /// Флаг, указывающий, что игрок находится в режиме ожидания решения о воскрешении.
    /// Когда этот флаг включен, игрок не уничтожается, пока игрок принимает решение.
    /// </summary>
    public struct PlayerThinkingFlag : IEnableableComponent, IComponentData { }

    [UpdateInGroup(typeof(DeathConsequencesGroup))]
    public partial struct PlayerReviveSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (revivesCount, currentHealth, maxHealth, entity) in
                     SystemAPI.Query<RefRW<RevivePlayerCount>, RefRW<CharacterCurrentHitPoints>, RefRW<CharacterMaxHitPoints>>()
                     .WithAll<DeathEntityFlag>()
                     .WithAll<PlayerTag>()
                     .WithEntityAccess())
            {
                if (revivesCount.ValueRW.Value > 0)
                {
                    revivesCount.ValueRW.Value--;
                    currentHealth.ValueRW.Value = maxHealth.ValueRW.Value;
                    SystemAPI.SetComponentEnabled<DeathEntityFlag>(entity, false);
                }
                else if (!revivesCount.ValueRW.IsAdvUsed)
                {
                    Debug.Log("Start revive");
                    revivesCount.ValueRW.IsAdvUsed = true;
                    GameUIController.Instance.SwitchDeathPanel();
                }
                else if(!SystemAPI.IsComponentEnabled<PlayerThinkingFlag>(entity))
                {
                    Debug.Log("Start Destroy");
                    SystemAPI.SetComponentEnabled<DestroyEntityFlag>(entity, true);
                }
            }
        }
    }
}
