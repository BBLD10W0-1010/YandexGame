using Unity.VisualScripting;
using UnityEngine;

public class SkillsStats : MonoBehaviour
{
    private static SkillsStats Instance;
    public static SkillsStats getInstance()
    {
        if (Instance == null)
            Instance = new SkillsStats();
        return Instance;
    }

    public float Damage { get; set; }
    public float Speed { get; set; }
    public float Health { get; set; }
}
