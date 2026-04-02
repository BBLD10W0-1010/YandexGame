using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Button quitButton;

    private void OnEnable() => quitButton.onClick.AddListener(GameUIController.Instance.QuitToMainMenu);
    private void OnDisable() => quitButton.onClick.RemoveListener(GameUIController.Instance.QuitToMainMenu);

    public void ShowCanvas()
    {
        panelRoot.SetActive(true);
    }
}
