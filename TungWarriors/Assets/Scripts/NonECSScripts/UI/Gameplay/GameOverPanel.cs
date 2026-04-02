using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Button quitButton;

    private void Awake() => panelRoot.SetActive(false);

    private void OnEnable() => quitButton.onClick.AddListener(GameUIController.Instance.QuitToMainMenu);
    private void OnDisable() => quitButton.onClick.RemoveListener(GameUIController.Instance.QuitToMainMenu);

    public void ShowDelayed()
    {
        StartCoroutine(ShowCoroutine());
    }

    private IEnumerator ShowCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        panelRoot.SetActive(true);
    }
}
