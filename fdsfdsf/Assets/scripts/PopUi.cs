using UnityEngine;
using UnityEngine.UI;

public class PopUi : UIWindow
{
    public override void Initialize()
    {
        base.Initialize();
        Button[] buttons = GetComponentsInChildren<Button>(true);
        GameplayManager gameplay = FindAnyObjectByType<GameplayManager>();
        foreach (var button in buttons)
        {
            string buttonName = button.name.ToLowerInvariant();
            if (buttonName.Contains("(1)") || buttonName.Contains("menu"))
                button.onClick.AddListener(ReturnToMenu);
            else
                button.onClick.AddListener(() => gameplay?.RestartGame());
        }
    }

    private void ReturnToMenu()
    {
        if (UiManager.Instance != null)
        {
            UiManager.Instance.HideWindow(Id);
            UiManager.Instance.HideWindow("GameplayUi");
            UiManager.Instance.ShowWindow("StartUi");
        }
    }
}
