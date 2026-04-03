using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

public partial struct UpdateGemUISystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (gemCount, shouldUpdateUI) in SystemAPI.Query<GemsCollectedCount, EnabledRefRW<UpdateGemUIFlag>>())
        {
            GameUIController.Instance.UpdateGemsCollectedText(gemCount.Value);
            shouldUpdateUI.ValueRW = false;
        }
    }
}

public partial struct PlayerWorldUISystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        foreach (var (uiPrefab, entity) in SystemAPI.Query<PlayerWorldUIPrefab>().WithNone<PlayerWorldUI>().WithEntityAccess())
        {
            var newWorldUI = Object.Instantiate(uiPrefab.Value.Value);
            ecb.AddComponent(entity, new PlayerWorldUI
            {
                CanvasTransform = newWorldUI.transform,
                HealthBarSlider = newWorldUI.GetComponentInChildren<Slider>()
            });
        }


        foreach (var (transform, worldUI, currentHitPoints, maxHitPoints) in SystemAPI.Query<LocalToWorld, PlayerWorldUI, CharacterCurrentHitPoints, CharacterMaxHitPoints>())
        {
            worldUI.CanvasTransform.Value.position = transform.Position;
            var healthValue = (float)currentHitPoints.Value / maxHitPoints.Value;
            worldUI.HealthBarSlider.Value.value = healthValue;
        }

        foreach (var (worldUI, entity) in SystemAPI.Query<PlayerWorldUI>().WithNone<LocalToWorld>().WithEntityAccess())
        {
            if (worldUI.CanvasTransform.Value != null)
            {
                Object.Destroy(worldUI.CanvasTransform.Value.gameObject);
            }

            ecb.RemoveComponent<PlayerWorldUI>(entity);
        }
        ecb.Playback(state.EntityManager);
    }
}