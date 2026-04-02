using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class EnemySpawnerAuthoring : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public float SpawnInterval;
    public float spawnDistance;
    public uint RandomSeed;
    
    private class Baker : Baker<EnemySpawnerAuthoring>
    {
        public override void Bake(EnemySpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new EnemySpawnData
            {
                EnemyPrefab = GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic),
                spawnInterval = authoring.SpawnInterval,
                spawnDistance = authoring.spawnDistance
            });
            AddComponent(entity, new EnemySpawnState
            {
                SpawnTimer = 0f,
                Random = Random.CreateFromIndex(authoring.RandomSeed)
            });
        }
    }
}
