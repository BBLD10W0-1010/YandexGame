using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

public class SpeedBuff : Buff
{
    public override EquipmentType[] Type => new EquipmentType[] { EquipmentType.Weapon, EquipmentType.Accessory, EquipmentType.Armor };

    public override float MinValue => (float)0.1;

    public override float MaxValue => (float) 1;

    public override float Value { get; set; }

    public override void Apply(Entity playerEntity)
    {
        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null || !world.EntityManager.Exists(playerEntity)) return;

        if (world.EntityManager.HasComponent<PlayerStats>(playerEntity))
        {
            var stats = world.EntityManager.GetComponentData<PlayerStats>(playerEntity);
            stats.BaseSpeed += Value;
            world.EntityManager.SetComponentData(playerEntity, stats);
        }
        else
        {
            Debug.LogWarning($"PlayerStats not found on entity {playerEntity}");
        }
    }
}
