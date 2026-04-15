
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct ApplySelectedLevelUpCardSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var entityManager = state.EntityManager;

        Entity playerEntity = Entity.Null;
        Entity selectedCardEntity = Entity.Null;

        foreach (var (selectedCard, availableCards, offeredCards, entity) in
                 SystemAPI.Query<RefRO<SelectedLevelUpCard>,
                         DynamicBuffer<AvailableLevelUpCardElement>,
                         DynamicBuffer<OfferedLevelUpCardElement>>()
                     .WithAll<PlayerTag>()
                     .WithEntityAccess())
        {
            if (!SystemAPI.IsComponentEnabled<SelectedLevelUpCard>(entity))
                continue;

            var selected = selectedCard.ValueRO.Value;

            entityManager.SetComponentData(entity, new SelectedLevelUpCard { Value = Entity.Null });
            entityManager.SetComponentEnabled<SelectedLevelUpCard>(entity, false);

            if (selected == Entity.Null || !Contains(offeredCards, selected))
                continue;

            playerEntity = entity;
            selectedCardEntity = selected;

            RemoveFromAvailable(availableCards, selected);
            offeredCards.Clear();
        }

        if (playerEntity == Entity.Null || selectedCardEntity == Entity.Null)
            return;

        ApplyCardEffects(entityManager, playerEntity, selectedCardEntity);

        if (GameUIController.Instance != null)
        {
            GameUIController.Instance.HideLevelUpPanel();
            GameUIController.Instance.TogglePause(false);
        }
    }

    private static bool Contains(DynamicBuffer<OfferedLevelUpCardElement> buffer, Entity e)
    {
        for (int i = 0; i < buffer.Length; i++)
            if (buffer[i].Value == e) return true;
        return false;
    }

    private static void RemoveFromAvailable(DynamicBuffer<AvailableLevelUpCardElement> buffer, Entity e)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            if (buffer[i].Value != e) continue;
            buffer.RemoveAt(i);
            return;
        }
    }

    private static void ApplyCardEffects(EntityManager em, Entity player, Entity card)
    {
        if (em.HasComponent<CardDamageBonusEffect>(card))
        {
            var bonus = em.GetComponentData<PlayerDamageBonus>(player);
            bonus.Value += em.GetComponentData<CardDamageBonusEffect>(card).Value;
            em.SetComponentData(player, bonus);
        }

        if (em.HasComponent<CardDefenseBonusEffect>(card))
        {
            var defense = em.GetComponentData<CharacterDefense>(player);
            defense.Value += em.GetComponentData<CardDefenseBonusEffect>(card).Value;
            em.SetComponentData(player, defense);
        }

        if (em.HasComponent<CardHealthRegenEffect>(card))
        {
            var regen = em.GetComponentData<CharacterHealthRegen>(player);
            regen.ValuePerSecond += em.GetComponentData<CardHealthRegenEffect>(card).ValuePerSecond;
            em.SetComponentData(player, regen);
        }

        if (em.HasComponent<CardMoveSpeedBonusEffect>(card))
        {
            var speedBonus = em.GetComponentData<CharacterMoveSpeedBonus>(player);
            speedBonus.Value += em.GetComponentData<CardMoveSpeedBonusEffect>(card).Value;
            em.SetComponentData(player, speedBonus);
        }

        if (em.HasComponent<CardUnlockBatWeaponEffect>(card) && !em.HasComponent<BatWeaponData>(player))
        {
            var effect = em.GetComponentData<CardUnlockBatWeaponEffect>(card);
            var batData = em.GetComponentData<BatWeaponData>(effect.BatPrefab);
            var batCooldown = em.GetComponentData<BatWeaponCooldown>(effect.BatPrefab);
            
            em.AddComponentData(player, batData);
            em.AddComponentData(player, batCooldown);

            Debug.Log("Bat weapon unlocked");
        }
    }
}