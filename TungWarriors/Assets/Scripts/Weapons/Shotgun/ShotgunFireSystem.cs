using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct ShotgunFireSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (transform, shotgunData, entity) in
                 SystemAPI.Query<RefRO<LocalTransform>, RefRW<ShotgunData>>().WithEntityAccess())
        {
            shotgunData.ValueRW.CooldownTimer -= deltaTime;

            bool shouldShoot = shotgunData.ValueRO.AutoFire == 1;

            if (!shouldShoot)
                continue;

            if (shotgunData.ValueRO.CooldownTimer > 0f)
                continue;

            shotgunData.ValueRW.CooldownTimer = shotgunData.ValueRO.Cooldown;

            int pelletCount = math.max(1, shotgunData.ValueRO.PelletCount);
            float totalSpreadRad = math.radians(shotgunData.ValueRO.SpreadAngle);

            // Если 1 снаряд — летит ровно по центру
            // Если 3 — углы будут примерно: -spread/2, 0, +spread/2
            for (int i = 0; i < pelletCount; i++)
            {
                float t = pelletCount == 1 ? 0.5f : (float)i / (pelletCount - 1);
                float angleOffset = math.lerp(-totalSpreadRad * 0.5f, totalSpreadRad * 0.5f, t);

                quaternion spreadRotation = quaternion.RotateZ(angleOffset);
                quaternion finalRotation = math.mul(transform.ValueRO.Rotation, spreadRotation);

                Entity plasmaBlast = ecb.Instantiate(shotgunData.ValueRO.PlasmaBlastPrefab);

                float3 forwardOffset = math.mul(finalRotation, new float3(1, 0, 0)) * shotgunData.ValueRO.SpawnOffset;

                ecb.SetComponent(plasmaBlast, LocalTransform.FromPositionRotation(
                    transform.ValueRO.Position + forwardOffset,
                    finalRotation
                ));
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}