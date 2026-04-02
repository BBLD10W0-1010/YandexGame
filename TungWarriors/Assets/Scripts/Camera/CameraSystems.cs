using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct CameraInitializationSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<InitializeCameraTargetTag>();
    }

    public void OnUpdate(ref SystemState state)
    {
        if (CameraTargetSingleton.Instance == null)
        {
            return;
        }
        var cameraTargetTransform = CameraTargetSingleton.Instance.transform;

        var entityCommandBuffer = new EntityCommandBuffer(state.WorldUpdateAllocator);

        foreach (var (cameraTarget, entity) in SystemAPI.Query<RefRW<CameraTarget>>().WithAll<InitializeCameraTargetTag, PlayerTag>().WithEntityAccess())
        {
            cameraTarget.ValueRW.CameraTransform = cameraTargetTransform;
            entityCommandBuffer.RemoveComponent<InitializeCameraTargetTag>(entity);
        }
        entityCommandBuffer.Playback(state.EntityManager);
    }
}

[UpdateAfter(typeof(TransformSystemGroup))]
public partial struct MoveCameraSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (entity, cameraTarget) in SystemAPI.Query<LocalToWorld, CameraTarget>().WithAll<PlayerTag>().WithNone<InitializeCameraTargetTag>())
        {
            cameraTarget.CameraTransform.Value.position = new Vector3(entity.Position.x, entity.Position.y, -10f);
        }
    }
}
