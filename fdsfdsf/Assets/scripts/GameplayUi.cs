using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;


public class GameplayUi : UIWindow
{
    [SerializeField] private Sprite settingsIcon;
    private Button settingsShortcut;
    private Button retreatButton;
    private GameplayManager manager;

    public override void Initialize()
    {
        base.Initialize();

        manager = FindFirstObjectByType<GameplayManager>();
        CreateShortcuts();
    }

    private void CreateShortcuts()
    {
        RectTransform root = CanvasRectTransform;
        if (root == null) return;
        settingsShortcut = RuntimeUiFactory.Button(root, string.Empty, new Vector2(-150, 280), new Vector2(76, 76), OpenSettings);
        settingsShortcut.gameObject.name = "Settings shortcut";
        if (settingsIcon != null)
        {
            var iconObject = new GameObject("Ajustes icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            iconObject.transform.SetParent(settingsShortcut.transform, false);
            var iconRect = (RectTransform)iconObject.transform;
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.offsetMin = new Vector2(6f, 6f);
            iconRect.offsetMax = new Vector2(-6f, -6f);
            var iconImage = iconObject.GetComponent<Image>();
            iconImage.sprite = settingsIcon;
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;
        }
        RuntimeUiFactory.Text(root, "AJUSTES", new Vector2(-150, 226), new Vector2(110, 24), 14).raycastTarget = false;
        retreatButton = RuntimeUiFactory.Button(root, "RETREAT", new Vector2(150, 280), new Vector2(190, 56), () => manager?.Retreat());
        retreatButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (retreatButton != null && manager != null)
            retreatButton.gameObject.SetActive(manager.IsRunning && manager.RemainingTime <= 30f && manager.RemainingTime > 0f);
    }

    private void OpenSettings()
    {
        SettingsUi settings = UiManager.Instance != null ? UiManager.Instance.GetWindow("SettingsUi")?.GetComponent<SettingsUi>() : null;
        settings?.ConfigureReturnToStart(false);
        UiManager.Instance?.ShowWindow("SettingsUi");
    }


}
