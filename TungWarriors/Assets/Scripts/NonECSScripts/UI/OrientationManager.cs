using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class OrientationManager : MonoBehaviour
{
    [SerializeField] private Canvas horizontalCanvas;
    [SerializeField] private Canvas verticalCanvas;

    private void Awake()
    {
        StartCoroutine(InitializeAfterDelay());
    }

    private IEnumerator InitializeAfterDelay()
    {
        yield return new WaitUntil(() => PlayerData.Instance != null);

        Debug.Log("Detect Orientation");
        DetectOrientation();
    }

    private void Update()
    {
        if (Screen.width > Screen.height)
        {
            if (!horizontalCanvas.gameObject.activeSelf)
                SwitchToHorizontal();
        }
        else
        {
            if (!verticalCanvas.gameObject.activeSelf)
                SwitchToVertical();
        }
    }

    private void DetectOrientation()
    {
        if (Screen.width > Screen.height)
            SwitchToHorizontal();
        else
            SwitchToVertical();
    }

    private void SwitchToHorizontal()
    {
        horizontalCanvas.gameObject.SetActive(true);
        verticalCanvas.gameObject.SetActive(false);
        Debug.Log("Switched to Horizontal UI");
    }

    private void SwitchToVertical()
    {
        horizontalCanvas.gameObject.SetActive(false);
        verticalCanvas.gameObject.SetActive(true);
        Debug.Log("Switched to Vertical UI");
    }
}
