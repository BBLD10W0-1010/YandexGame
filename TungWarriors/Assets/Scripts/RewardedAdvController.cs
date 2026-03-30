using Assets.Scripts.DeathConsequencesSystems;
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

        switch (rewardId)
        {
            case(RewardedAdvAwards.extraLife):
                YG2.RewardedAdvShow(rewardId.ToString(), () =>
                {
                    if (world == null || !world.IsCreated) return;

                    var playerQuery = world.EntityManager.CreateEntityQuery(
                        typeof(CharacterCurrentHitPoints),
                        typeof(CharacterMaxHitPoints),
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

                        world.EntityManager.SetComponentData(playerEntity, newCurrentHitPoints);
                        world.EntityManager.SetComponentEnabled<DeathEntityFlag>(playerEntity, false);

                        UnityEngine.Debug.Log("End Revive");
                        
                        playerQuery.Dispose();
                    }
                    GameUIController.Instance.SwitchDeathPanel();
                });
                break;
        }
        
    }
}