using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class StartUi : UIWindow
{
    [Header("Botones")]
    public Button gameplayButton;
    public Button creditsButton;
    public Button settingsButton;

    [Header("Nombres de las ventanas")]
    [SerializeField] private string gameplayWindow = "GameplayUi";
    [SerializeField] private string creditsWindow = "CreditsUi";
    [SerializeField] private string settingsWindow = "SettingsUi";

    [SerializeField] private float delay = 1f;

    public override void Initialize()
    {
        base.Initialize();

        if (gameplayButton != null)
            gameplayButton.onClick.AddListener(() => OpenWindow(gameplayWindow));
        else
            Debug.LogWarning("Falta asignar el boton de Gameplay.");

        if (creditsButton != null)
            creditsButton.onClick.AddListener(() => OpenWindow(creditsWindow));
        else
            Debug.LogWarning("Falta asignar el boton de Creditos.");

        if (settingsButton != null)
            settingsButton.onClick.AddListener(() => OpenWindow(settingsWindow));
        else
            Debug.LogWarning("Falta asignar el boton de Settings.");
    }

    private void OpenWindow(string windowName)
    {
        StartCoroutine(SwitchWindow(windowName));
    }

    private IEnumerator SwitchWindow(string windowName)
    {
        UiManager.Instance.HideWindow("StartUi");
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
