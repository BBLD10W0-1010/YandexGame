using Unity.Entities;
using UnityEngine;

public struct BatAttackData : IComponentData
{
    public float HitPoints;
    public float CooldownTime;
    public float Speed;
    public Entity attackPrefab;
}
//using Unity.Entities;

//public struct EnemyTag : IComponentData { }

//public struct EnemyAttackData : IComponentData
//{
//    public int HitPoints;
//    public float CooldownTime;
//}

//public struct EnemyCooldownExpirationTimestamp : IComponentData, IEnableableComponent
//{
//    public double value;
//}

//public struct GemPrefab : IComponentData
//{
//    public Entity Value;
//}