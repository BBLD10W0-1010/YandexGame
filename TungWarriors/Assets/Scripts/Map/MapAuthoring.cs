using Unity.Entities;
using UnityEngine;

public class MapAuthoring : MonoBehaviour
{
    public GameObject TilePrefab;
    [Tooltip("Лучше ставить нечетные числа")]
    public float TileSize;
    [Range(3, 13)]
    public int GridDimension;

    private class Baker : Baker<MapAuthoring>
    {
        public override void Bake(MapAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new MapSettings
            {
                TilePrefab = GetEntity(authoring.TilePrefab, TransformUsageFlags.Dynamic),
                TileSize = authoring.TileSize,
                GridDimension = authoring.GridDimension
            });
            AddComponent<InitializeMapFlag>(entity);
        }
    }
}