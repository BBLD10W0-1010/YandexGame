using System;
using Unity.Entities;
using UnityEngine;
using YG; // PluginYG2 пространство имен

public enum RewardedAdvAwards
{
    extraLife,
    coins
}

public class RewardedAdvController : MonoBehaviour
{
    public static RewardedAdvController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("RewardedAdvController already exists, destroying duplicate");
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ShowRewardedAdv(RewardedAdvAwards rewardId)
    {
        if (YG2.nowRewardAdv)
        {
            return;
        }

        var world = World.DefaultGameObjectInjectionWorld;

        switch(rewardId)
        {
            case(RewardedAdvAwards.extraLife):
                YG2.RewardedAdvShow(rewardId.ToString(), () =>
                {
                    if (world == null || !world.IsCreated) return;

                    var playerQuery = world.EntityManager.CreateEntityQuery(
                        typeof(CharacterCurrentHitPoints),
                        typeof(CharacterMaxHitPoints),
                        typeof(RevivePlayerCount),
                        typeof(DestroyEntityFlag),
                        typeof(PlayerTag)
                    );

                    if (!playerQuery.IsEmpty)
                    {
                        var playerEntity = playerQuery.GetSingletonEntity();
                        var maxHitPoints = world.EntityManager.GetComponentData<CharacterMaxHitPoints>(playerEntity);

                        var newCurrentHitPoints = new CharacterCurrentHitPoints
                        {
                            Value = maxHitPoints.Value
                        };

                        var newRevivePlayerCount = world.EntityManager.GetComponentData<RevivePlayerCount>(playerEntity);
                        newRevivePlayerCount.IsAdvUsed = true;

                        world.EntityManager.SetComponentData(playerEntity, newCurrentHitPoints);
                        world.EntityManager.SetComponentData(playerEntity, newRevivePlayerCount);

                        world.EntityManager.SetComponentEnabled<DestroyEntityFlag>(playerEntity, false);

                        playerQuery.Dispose();
                    }
                });
                break;
        }
        
    }
}