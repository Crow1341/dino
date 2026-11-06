using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class GabilitiesUi : UIWindow
{
    [Header("Botones")]
    public Button backButton;

    [Header("Nombres de las ventanas")]
    [SerializeField] private string gameplayWindow = "GameplayUi";

    [SerializeField] private float delay = 1f;

    public override void Initialize()
    {
        base.Initialize();

        backButton.onClick.AddListener(() => OpenWindow(gameplayWindow));
    }

    private void OpenWindow(string windowName)
    {
        StartCoroutine(SwitchWindow(windowName));
    }

    private IEnumerator SwitchWindow(string windowName)
    {
        UiManager.Instance.HideWindow("HabilitiesUi");
        yield return new WaitForSeconds(delay);
        UiManager.Instance.ShowWindow(windowName);
    }

    #region Test Methods

    [Button("Test Show")]
    private void TestShow()
    {
        Show();
    }

    [Button("Test Hide")]
    private void TestHide()
    {
        Hide();
    }

    #endregion
}
