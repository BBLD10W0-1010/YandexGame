using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

public class Equipment
{
    public string Name { get; set; }
    public Sprite Icon { get; set; }
    public EquipmentType Type { get; set; }
    public List<Buff> Buffs { get; set; } = new List<Buff>();
    public int Cost => Buffs.Count * 15;
    public void ApplyToPlayer(Entity playerEntity)
    {
        foreach (var buff in Buffs)
        {
            buff.Apply(playerEntity);
        }
    }
}