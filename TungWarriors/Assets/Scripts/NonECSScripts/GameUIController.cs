using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameUIController : MonoBehaviour
{
    public static GameUIController Instance { get; private set; }
    [SerializeField] private HUDPanel hudPanel;
    [SerializeField] private PausePanel pausePanel;
    [SerializeField] private GameOverPanel gameOverPanel;
    [SerializeField] private RevivePanel revivePanel;

    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Multiple GameUIController instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void TogglePause(bool pause)
    {
        IsPaused = pause;
        var defaultWorld = World.DefaultGameObjectInjectionWorld;
        if (defaultWorld != null && defaultWorld.IsCreated)
        {
            var initSystemGroup = defaultWorld.GetExistingSystemManaged<InitializationSystemGroup>();
            if (initSystemGroup != null)
            {
                initSystemGroup.Enabled = !IsPaused;
            }
        }
    }

    public void QuitToMainMenu()
    {
        TogglePause(false);
        SceneManager.LoadScene("MainMenu");
    }

    public void UpdateGemsCollectedText(int gems)
    {
        hudPanel.UpdateGemsText(gems);
    }

    public void ShowGameOverUI()
    {
        StartCoroutine(ShowGameOverUICoroutine());
    }

    private IEnumerator ShowGameOverUICoroutine()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        gameOverPanel.ShowCanvas();
    }
    public void SwitchDeathPanel()
    {
        revivePanel.TogglePanel();
    }

    public void ShowPauseMenu()
    {
        TogglePause(true);
        pausePanel.Show();
    }
}