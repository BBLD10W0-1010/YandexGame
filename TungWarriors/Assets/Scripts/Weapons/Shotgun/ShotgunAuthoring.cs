using UnityEngine;
using Unity.Entities;

public class ShotgunAuthoring : MonoBehaviour
{
    public GameObject PlasmaBlastPrefab;
    public float Cooldown = 0.5f;
    public int PelletCount = 3;
    public float SpreadAngle = 20f;   // полный угол конуса
    public float SpawnOffset = 1.0f;  // насколько впереди спавнить
    public bool AutoFire = true;      // для теста: стреляет автоматически

    private class Baker : Baker<ShotgunAuthoring>
    {
        public override void Bake(ShotgunAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new ShotgunData
            {
                PlasmaBlastPrefab = GetEntity(authoring.PlasmaBlastPrefab, TransformUsageFlags.Dynamic),
                Cooldown = authoring.Cooldown,
                CooldownTimer = 0f,
                PelletCount = authoring.PelletCount,
                SpreadAngle = authoring.SpreadAngle,
                SpawnOffset = authoring.SpawnOffset,
                AutoFire = authoring.AutoFire ? (byte)1 : (byte)0
            });
        }
    }
}

