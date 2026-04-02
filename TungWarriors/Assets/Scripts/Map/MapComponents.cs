using Unity.Entities;

public struct MapTileTag : IComponentData { }

public struct MapSettings : IComponentData
{
    public Entity TilePrefab;
    public float TileSize;
    public int GridDimension;
}

public struct InitializeMapFlag : IComponentData, IEnableableComponent { }