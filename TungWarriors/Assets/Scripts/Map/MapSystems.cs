using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/// <summary>
/// Система создает произвольную карту из уже имеющегося тайла со случайным поворотом.
/// Строится сначала в ширину и постепенно сдвигается вправо вдоль ОХ. Пока что так =)
/// </summary>
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct MapInitializationSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        foreach (var (settings, initFlag) in SystemAPI.Query<RefRO<MapSettings>, EnabledRefRW<InitializeMapFlag>>())
        {
            initFlag.ValueRW = false;
            var tileSize = settings.ValueRO.TileSize;
            var halfGrid = settings.ValueRO.GridDimension / 2;
            for (int x = -halfGrid; x <= halfGrid; x++)
            {
                for (int y = -halfGrid; y <= halfGrid; y++)
                {
                    var newTile = ecb.Instantiate(settings.ValueRO.TilePrefab);
                    var pos2D = new float2(x * tileSize, y * tileSize);
                    ecb.SetComponent(newTile, LocalTransform.FromPosition(new float3(pos2D, 0f)));
                    ecb.AddComponent<MapTileTag>(newTile);
                }
            }
        }
        ecb.Playback(state.EntityManager);
    }
}

/// <summary>
/// Начинаем перестроечку, если игрок приблизился к половине длины/ширины карты
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct InfiniteMapSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonEntity<PlayerTag>(out Entity playerEntity) ||
        !SystemAPI.TryGetSingleton<MapSettings>(out MapSettings mapSettings))
            return;
        var playerPos3D = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position;
        var gridWidth = mapSettings.TileSize * mapSettings.GridDimension;
        state.Dependency = new RelocateMapTilesJob
        {
            PlayerPos = playerPos3D.xy,
            HalfExtent = gridWidth * 0.5f,
            GridWidth = gridWidth
        }.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
[WithAll(typeof(MapTileTag))]
public partial struct RelocateMapTilesJob : IJobEntity
{
    public float2 PlayerPos;
    public float HalfExtent;
    public float GridWidth;

    public void Execute(ref LocalTransform transform)
    {
        var tilePos = transform.Position.xy;
        var diff = PlayerPos - tilePos;
        var newPos = tilePos;
        if (diff.x > HalfExtent)
            newPos.x += GridWidth;
        else if (diff.x < -HalfExtent)
            newPos.x -= GridWidth;
        if (diff.y > HalfExtent)
            newPos.y += GridWidth;
        else if (diff.y < -HalfExtent)
            newPos.y -= GridWidth;
        if (math.any(newPos != tilePos))
        {
            var hash = math.hash(new int2((int)newPos.x, (int)newPos.y));
            transform.Position = new float3(newPos, 0f);
        }
    }
}