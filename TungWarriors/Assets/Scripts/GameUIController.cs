using System.Collections;
using TMPro;
using Unity.Entities;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class GameUIController : MonoBehaviour
{
    public static GameUIController Instance;

    [Header("In the Game")]
    [SerializeField] private TextMeshProUGUI gemsCollected;
    [SerializeField] private Button pauseButton;

    [Header("GameOver Menu")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button quitButton;
    
    [Header("Pause Menu")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button pauseQuitButton;

    [Header("Preparring to death")]
    [SerializeField] private GameObject preparingToDeathPanel;
    [SerializeField] private Button consentToAdvButton;
    [SerializeField] private Button rejectOfAdvButton;

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("There is multiple Instances of GameUIController. Destroying new instance", Instance);
            return;
        }
        Instance = this;
        UpdateGemsCollectedText(0);
    }

    private void Start()
    {
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        preparingToDeathPanel.SetActive(false);
    }

    private void OnEnable()
    {
        pauseButton.onClick.AddListener(OnPauseButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
        resumeButton.onClick.AddListener(OnPauseButtonClicked);
        pauseQuitButton.onClick.AddListener(OnQuitButtonClicked);
        consentToAdvButton.onClick.AddListener(OnConsentToAdvButton);
        rejectOfAdvButton.onClick.AddListener(OnRejectOfAdvButton);
    }

    private void OnDisable()
    {
        pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
        quitButton.onClick.RemoveListener(OnQuitButtonClicked);
        resumeButton.onClick.RemoveListener(OnPauseButtonClicked);
        pauseQuitButton.onClick.RemoveListener(OnQuitButtonClicked);
        consentToAdvButton.onClick.RemoveListener(OnConsentToAdvButton);
        rejectOfAdvButton.onClick.RemoveListener(OnRejectOfAdvButton);
    }

    private void SetEcsEnabled(bool shouldEnable) 
    {
        var defaultWorld = World.DefaultGameObjectInjectionWorld;
        if (defaultWorld == null)
        {
            return;
        }

        var initSystemGroup = defaultWorld.GetExistingSystemManaged<InitializationSystemGroup>();
        initSystemGroup.Enabled = shouldEnable;
    }

    public void PauseGame(bool onPaus)
    {
        isPaused = onPaus;
        SetEcsEnabled(!isPaused);
    }

    public void UpdateGemsCollectedText(int gems)
    {
        gemsCollected.text = $"{gems}";
    }

    public void ShowGameOverUI()
    {
        StartCoroutine(ShowGameOverUICoroutine());
    }

    private IEnumerator ShowGameOverUICoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        gameOverPanel.SetActive(true);
    }

    public void SwitchDeathPanel()
    {
        PauseGame(!isPaused);
        preparingToDeathPanel.SetActive(!preparingToDeathPanel.activeInHierarchy);
    }

    #region OnButtonsClicked

    private void OnQuitButtonClicked()
    {
        SetEcsEnabled(true);
        SceneManager.LoadScene("MainMenu");
    }

    private void OnPauseButtonClicked()
    {
        PauseGame(!isPaused);
        pausePanel.SetActive(isPaused);
    }

    private void OnConsentToAdvButton()
    {
        RewardedAdvController.Instance.ShowRewardedAdv(RewardedAdvAwards.extraLife);
        SwitchDeathPanel();
    }

    private void OnRejectOfAdvButton()
    {
        SwitchDeathPanel();
    }
    #endregion
}