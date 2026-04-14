using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

public abstract class Buff
{
    public abstract EquipmentType[] Type { get; }
    public abstract float MinValue { get; }
    public abstract float MaxValue { get; }
    public abstract float Value { get; set; }
    public abstract void Apply(Entity playerEntity);
}
