using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BatVisualController : MonoBehaviour
{
    [SerializeField] private Transform batVisual;
    [SerializeField] private SpriteRenderer batSpriteRenderer;

    private EntityManager _entityManager;
    private EntityQuery _playerQuery;
    private float _swingTimer;

    private void Start()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null || !world.IsCreated)
        {
            enabled = false;
            return;
        }


        _entityManager = world.EntityManager;
        _playerQuery = _entityManager.CreateEntityQuery(ComponentType.ReadOnly<PlayerTag>());

        if (batSpriteRenderer != null)
        {
            batSpriteRenderer.enabled = false;
        }
    }

    private void LateUpdate()
    {
        if (_playerQuery.IsEmptyIgnoreFilter || batVisual == null || batSpriteRenderer == null)
            return;

        var playerEntity = _playerQuery.GetSingletonEntity();
        var hasBat = _entityManager.HasComponent<BatWeaponData>(playerEntity);

        if (!hasBat)
        {
            batSpriteRenderer.enabled = false;
            return;
        }

        var playerPos = _entityManager.GetComponentData<Unity.Transforms.LocalToWorld>(playerEntity).Position;
        batVisual.position = new Vector3(playerPos.x, playerPos.y, 0f);
        batSpriteRenderer.enabled = true;

        var batData = _entityManager.GetComponentData<BatWeaponData>(playerEntity);

        var direction = new float2(1f, 0f);
        if (_entityManager.HasComponent<LastNonZeroMoveDirection>(playerEntity))
        {
            var lastDirection = _entityManager.GetComponentData<LastNonZeroMoveDirection>(playerEntity).Value;
            if (math.lengthsq(lastDirection) > 0.0001f)
            {
                direction = math.normalize(lastDirection);
            }
        }

        var baseAngle = math.atan2(direction.y, direction.x);
        var halfConeRadians = math.radians(batData.ConeAngleDegrees * 0.5f);
        var swingPeriod = math.max(0.01f, batData.Cooldown);

        _swingTimer += Time.deltaTime;
        while (_swingTimer > swingPeriod)
        {
            _swingTimer -= swingPeriod;
        }

        var phase = _swingTimer / swingPeriod;

        float swingNormalized;
        if (phase < 0.5f)
            swingNormalized = math.lerp(-1f, 1f, phase * 2f);
        else
        {
            swingNormalized = math.lerp(1f, -1f, (phase - 0.5f) * 2f);
        }

        var currentAngle = baseAngle + swingNormalized * halfConeRadians;
        batVisual.localPosition = Vector3.zero;
        batVisual.localRotation = Quaternion.Euler(0f, 0f, math.degrees(currentAngle));
    }
}