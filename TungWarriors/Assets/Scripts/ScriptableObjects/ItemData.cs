using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Item")]
public class ItemData : ScriptableObject
{
    public Sprite img;
    public string name;
    public EquipmentType type;
}
