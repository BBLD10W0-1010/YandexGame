using Unity.Entities;
using UnityEngine;

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
                     .WithAll<DeathEntityFlag>()
                     .WithEntityAccess())
            {
               Debug.Log("Enemy death");
               SystemAPI.SetComponentEnabled<DestroyEntityFlag>(entity, true);
            }
        }
    }
}
