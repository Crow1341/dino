using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class Credits : UIWindow
{
    [Header("Botones")]
    public Button backButton;
   

    [Header("Nombres de las ventanas")]
    [SerializeField] private string startWindow = "StartUi";
   

    [SerializeField] private float delay = 1f;

    public override void Initialize()
    {
        base.Initialize();

        backButton.onClick.AddListener(() => OpenWindow(startWindow));
        
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
