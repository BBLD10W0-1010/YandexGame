using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct ShowLevelUpSelectionSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        if (GameUIController.Instance == null)
            return;

        var entityManager = state.EntityManager;
        foreach (var (randomState, availableCards, offeredCards, entity) in
                 SystemAPI.Query<RefRW<PlayerCardRandom>,
                         DynamicBuffer<AvailableLevelUpCardElement>,
                         DynamicBuffer<OfferedLevelUpCardElement>>()
                     .WithAll<PlayerTag>()
                     .WithEntityAccess())
        {
            var flagEnabled = SystemAPI.IsComponentEnabled<ShowLevelUpSelectionFlag>(entity);
            if (!SystemAPI.IsComponentEnabled<ShowLevelUpSelectionFlag>(entity))
                continue;
            var hasFlag = SystemAPI.HasComponent<ShowLevelUpSelectionFlag>(entity);
            SystemAPI.SetComponentEnabled<ShowLevelUpSelectionFlag>(entity, false);
            if (availableCards.Length == 0)
                continue;
            offeredCards.Clear();
            var tempPool = new NativeList<Entity>(availableCards.Length, Allocator.Temp);
            for (int i = 0; i < availableCards.Length; i++)
                tempPool.Add(availableCards[i].Value);
            var random = randomState.ValueRW.Value;
            var offerCount = math.min(3, tempPool.Length);
            for (int i = 0; i < offerCount; i++)
            {
                var idx = random.NextInt(0, tempPool.Length);
                offeredCards.Add(new OfferedLevelUpCardElement { Value = tempPool[idx] });
                tempPool.RemoveAtSwapBack(idx);
            }

            randomState.ValueRW.Value = random;
            tempPool.Dispose();

            var cardsToShow = new List<LevelUpCardViewData>(offeredCards.Length);
            for (int i = 0; i < offeredCards.Length; i++)
            {
                var meta = entityManager.GetComponentData<LevelUpCardMeta>(offeredCards[i].Value);
                cardsToShow.Add(new LevelUpCardViewData(
                    offeredCards[i].Value,
                    meta.Title.ToString(),
                    meta.Description.ToString(),
                    meta.Icon.Value
                ));
            }
            GameUIController.Instance.ShowLevelUpPanel(cardsToShow);
        }
    }
}