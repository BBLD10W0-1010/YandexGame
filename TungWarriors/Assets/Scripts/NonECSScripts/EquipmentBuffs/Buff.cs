using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

public abstract class Buff
{
    private float _value;
    public float Value
    {
        get => (float)Math.Round(_value, 2);
        set => _value = value;
    }

    public abstract EquipmentType[] Type { get; }
    public abstract float MinValue { get; }
    public abstract float MaxValue { get; }
    
    public abstract string Description { get;}
    public abstract void Apply(Entity playerEntity);
}
