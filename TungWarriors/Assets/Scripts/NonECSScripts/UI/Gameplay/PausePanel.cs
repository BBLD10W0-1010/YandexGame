using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;

    private void OnEnable()
    {
        resumeButton.onClick.AddListener(Hide);
        quitButton.onClick.AddListener(GameUIController.Instance.QuitToMainMenu);
    }

    private void OnDisable()
    {
        resumeButton.onClick.RemoveListener(Hide);
        quitButton.onClick.RemoveListener(GameUIController.Instance.QuitToMainMenu);
    }

    public void Show() => panelRoot.SetActive(true);

    private void Hide()
    {
        panelRoot.SetActive(false);
        GameUIController.Instance.TogglePause(false);
    }
}
