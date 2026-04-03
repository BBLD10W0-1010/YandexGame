using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gemsCollected;
    [SerializeField] private Button pauseButton;

    private void OnEnable() => pauseButton.onClick.AddListener(OnPauseClicked);
    private void OnDisable() => pauseButton.onClick.RemoveListener(OnPauseClicked);

    public void UpdateGemsText(int gems)
    {
        gemsCollected.text = $"{gems}";
    }

    private void OnPauseClicked()
    {
        GameUIController.Instance.ShowPauseMenu();
    }
}
