using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NavigationPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject shopPanel;

    [SerializeField] private Button toMainMenuPanel;
    [SerializeField] private Button toInventoryPanel;
    [SerializeField] private Button toShopPanel;

    private GameObject currentPanel;

    private void Awake()
    {
        currentPanel = mainMenuPanel;
        mainMenuPanel.SetActive(true);
        inventoryPanel.SetActive(false);
        shopPanel.SetActive(false);
    }

    private void OnEnable()
    {
        toMainMenuPanel.onClick.AddListener(ChangePanelToMainMenu);
        toInventoryPanel.onClick.AddListener(ChangePanelToInventory);
        toShopPanel.onClick.AddListener(ChangePanelToShop);
    }

    private void OnDisable()
    {
        toMainMenuPanel.onClick.RemoveListener(ChangePanelToMainMenu);
        toInventoryPanel.onClick.RemoveListener(ChangePanelToInventory);
        toShopPanel.onClick.RemoveListener(ChangePanelToShop);
    }

    private void ChangePanel(GameObject panel)
    {
        panel.SetActive(true);
        currentPanel.SetActive(false);
        currentPanel = panel;
    }

    private void ChangePanelToMainMenu()
    {
        ChangePanel(mainMenuPanel);
    }

    private void ChangePanelToInventory()
    {
        ChangePanel(inventoryPanel);
    }

    private void ChangePanelToShop()
    {
        ChangePanel(shopPanel);
    }
}

