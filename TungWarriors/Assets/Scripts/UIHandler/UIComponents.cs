using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public struct UpdateGemUIFlag : IComponentData, IEnableableComponent { }

public struct PlayerWorldUI : ICleanupComponentData
{
    public UnityObjectRef<Transform> CanvasTransform;
    public UnityObjectRef<Slider> HealthBarSlider;
}

public struct PlayerWorldUIPrefab : IComponentData
{
    public UnityObjectRef<GameObject> Value;
}
