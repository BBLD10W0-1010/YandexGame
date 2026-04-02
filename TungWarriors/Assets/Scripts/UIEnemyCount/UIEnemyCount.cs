using TMPro;
using Unity.Entities;
using UnityEngine;

public class EnemyCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;

    private EntityManager _entityManager;
    private EntityQuery _spawnQuery;

    private int _lastCurrent = -1;
    private int _lastMax = -1;

    private void Start()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null)
        {
            Debug.LogWarning("Default ECS World not found");
            enabled = false;
            return;
        }

        _entityManager = world.EntityManager;
        _spawnQuery = _entityManager.CreateEntityQuery(ComponentType.ReadOnly<EnemySpawnState>());
    }

    private void Update()
    {
        if (!_spawnQuery.IsEmptyIgnoreFilter)
        {
            var spawnState = _spawnQuery.GetSingleton<EnemySpawnState>();

            if (spawnState.CurrentSpawnedEnemies != _lastCurrent || spawnState.MaxSpawnedEnemies != _lastMax)
            {
                _lastCurrent = spawnState.CurrentSpawnedEnemies;
                _lastMax = spawnState.MaxSpawnedEnemies;

                counterText.text = $"{_lastCurrent}/{_lastMax}";
            }
        }
        else
        {
            counterText.text = "0/0";
        }
    }
}