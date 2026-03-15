using System.Collections;
using TMPro;
using Unity.Entities;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    public static GameUIController Instance;


    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private Button _quitButton;
    [SerializeField] private TextMeshProUGUI _gemsCollected;

    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _pauseQuitButton;

    private bool _isPaused = false;

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


    private void OnEnable()
    {
        _quitButton.onClick.AddListener(OnQuitButtonClicked);
        _resumeButton.onClick.AddListener(OnResumeButtonClicked);
        _pauseQuitButton.onClick.AddListener(OnQuitButtonClicked);
    }
    private void OnDisable()
    {
        _quitButton.onClick.RemoveListener(OnQuitButtonClicked);
        _resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
        _pauseQuitButton.onClick.RemoveListener(OnQuitButtonClicked);
    }

    private void Start()
    {
        _gameOverPanel.SetActive(false);
        _pausePanel.SetActive(false);
    }

    private void Update()
    {
    }

    private void ToggleGamePause()
    {
        _isPaused = !_isPaused;
        _pausePanel.SetActive(_isPaused);
        SetEcsEnabled(!_isPaused);
    }

    private void SetEcsEnabled(bool shouldEnable) 
    {
        var defaultWorld = World.DefaultGameObjectInjectionWorld;
        if (defaultWorld != null)
        {
            return;
        }

        var initSystemGroup = defaultWorld.GetExistingSystemManaged<InitializationSystemGroup>();
        initSystemGroup.Enabled = shouldEnable;
    }

    public void UpdateGemsCollectedText(int gemsCollected)
    {
        _gemsCollected.text = $"Gems Collected: {gemsCollected}";
    }

    public void ShowGameOverUI()
    {
        StartCoroutine(ShowGameOverUICoroutine());
    }

    private IEnumerator ShowGameOverUICoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        _gameOverPanel.SetActive(true);
    }

    private void OnResumeButtonClicked()
    {
        ToggleGamePause();
    }

    private void OnQuitButtonClicked()
    {
        SetEcsEnabled(true);
        SceneManager.LoadScene("MainMenu");
    }
}
