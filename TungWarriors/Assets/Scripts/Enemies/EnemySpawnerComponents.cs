using Unity.Entities;
using Random = Unity.Mathematics.Random;

public struct EnemySpawnData : IComponentData
{
    public Entity EnemyPrefab;
    public float spawnInterval;
    public float spawnDistance;
}

public struct EnemySpawnState : IComponentData
{
    public float SpawnTimer;
    public int CurrentSpawnedEnemies;
    public int MaxSpawnedEnemies;
    public Random Random;
}