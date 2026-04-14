using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set; }

    [Header("Resources")]
    [SerializeField] private int gold;
    [SerializeField] private int gems;
    [SerializeField] private int rubies;

    [Header("Inventory")]
    [SerializeField] private List<Equipment> inventory = new List<Equipment>();
    [SerializeField] private List<Equipment> currentEquipment;

    public event Action<int> OnGoldChanged;
    public event Action<int> OnGemsChanged;
    public event Action OnInventoryChanged;

    public int Gold
    {
        get => gold;
        set
        {
            gold = Mathf.Max(0, value);
            OnGoldChanged?.Invoke(gold);
        }
    }

    public int Gems
    {
        get => gems;
        set
        {
            gems = Mathf.Max(0, value);
            OnGemsChanged?.Invoke(gems);
        }
    }

    public List<Equipment> Inventory => inventory;
    public List<Equipment> CurrentEquipment
    {
        get => currentEquipment;
        set
        {
            currentEquipment = value;
            OnInventoryChanged?.Invoke();
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}