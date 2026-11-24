using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;


public class GameplayUi : UIWindow
{
    [SerializeField] private Sprite settingsIcon;
    private Button settingsShortcut;
    private Button retreatButton;
    private GameplayManager manager;
    private RectTransform hudRoot;

    public RectTransform EnsureHudRoot()
    {
        if (hudRoot != null)
            return hudRoot;

        RectTransform root = CanvasRectTransform;
        if (root == null)
            return null;

        Transform existing = root.Find("Gameplay HUD");
        if (existing != null)
        {
            hudRoot = existing as RectTransform;
            return hudRoot;
        }

        var hudObject = new GameObject("Gameplay HUD", typeof(RectTransform), typeof(Canvas));
        hudObject.transform.SetParent(root, false);
        hudRoot = (RectTransform)hudObject.transform;
        hudRoot.anchorMin = Vector2.zero;
        hudRoot.anchorMax = Vector2.one;
        hudRoot.offsetMin = Vector2.zero;
        hudRoot.offsetMax = Vector2.zero;

        Canvas hudCanvas = hudObject.GetComponent<Canvas>();
        hudCanvas.overrideSorting = true;
        hudCanvas.sortingOrder = 20;
        return hudRoot;
    }

    public override void Initialize()
    {
        base.Initialize();

        manager = FindFirstObjectByType<GameplayManager>();
        CreateShortcuts();
    }

    private void CreateShortcuts()
    {
        RectTransform root = EnsureHudRoot();
        if (root == null) return;
        settingsShortcut = RuntimeUiFactory.Button(root, string.Empty, Vector2.zero, new Vector2(62, 62), OpenSettings);
        settingsShortcut.gameObject.name = "Settings shortcut";
        PlaceAtTopRight(settingsShortcut.GetComponent<RectTransform>(), new Vector2(-32f, -28f));
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
        TMP_Text settingsLabel = RuntimeUiFactory.Text(root, "AJUSTES", Vector2.zero, new Vector2(96, 22), 13);
        settingsLabel.raycastTarget = false;
        PlaceAtTopRight(settingsLabel.rectTransform, new Vector2(-32f, -88f));
        retreatButton = RuntimeUiFactory.Button(root, "RETIRARSE", Vector2.zero, new Vector2(150, 52), () => manager?.Retreat());
        PlaceAtTopRight(retreatButton.GetComponent<RectTransform>(), new Vector2(-112f, -32f));
        retreatButton.gameObject.SetActive(false);
    }

    private static void PlaceAtTopRight(RectTransform rect, Vector2 position)
    {
        rect.anchorMin = Vector2.one;
        rect.anchorMax = Vector2.one;
        rect.pivot = Vector2.one;
        rect.anchoredPosition = position;
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
