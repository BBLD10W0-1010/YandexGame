using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace Assets.Scripts.DeathConsequencesSystems
{
    [UpdateInGroup(typeof(DeathConsequencesGroup), OrderLast = true)]
    public partial struct EnemyDeathSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (_, entity) in
                     SystemAPI.Query<DeathEntityFlag>()
                     .WithAll<EnemyTag>()
                     .WithPresent<DeathEntityFlag>()
                     .WithEntityAccess())
            {
               SystemAPI.SetComponentEnabled<DestroyEntityFlag>(entity, true);
            }
        }
    }
}
