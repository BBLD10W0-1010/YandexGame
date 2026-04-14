using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanelManager : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _exitGame;

    private void OnEnable()
    {
        _exitGame.onClick.AddListener(OnExitButtonClicked);
        _playButton.onClick.AddListener(OnPlayButtonClicked);
    }

    private void OnDisable()
    {
        _exitGame.onClick.RemoveListener(OnExitButtonClicked);
        _playButton.onClick.RemoveListener(OnPlayButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("DefaultLevel");
    }

    private void OnExitButtonClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
