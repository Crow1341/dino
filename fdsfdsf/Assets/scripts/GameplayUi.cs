using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;


public class GameplayUi : UIWindow
{
    [SerializeField] private Sprite settingsIcon;
    private Button settingsShortcut;
    private Button retreatButton;
    private GameplayManager manager;
    private RectTransform hudRoot;
    private TMP_Text defeatedMonstersText;
    private RectTransform pauseMenu;
    private CanvasGroup pauseMenuGroup;

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

        var hudObject = new GameObject("Gameplay HUD", typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster));
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
        CreateDefeatedMonstersCounter();
        CreatePauseMenu();
        if (manager != null)
        {
            manager.MonstersDefeatedChanged += RefreshDefeatedMonstersCounter;
            RefreshDefeatedMonstersCounter(manager.MonstersDefeated);
        }
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
        TMP_Text settingsLabel = RuntimeUiFactory.Text(root, "PAUSA", Vector2.zero, new Vector2(96, 22), 13);
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

    private void CreateDefeatedMonstersCounter()
    {
        RectTransform root = EnsureHudRoot();
        if (root == null || defeatedMonstersText != null)
            return;

        defeatedMonstersText = RuntimeUiFactory.Text(root, "Monstruos derrotados: 0", Vector2.zero, new Vector2(420f, 38f), 22);
        defeatedMonstersText.name = "Defeated Monsters Counter";
        defeatedMonstersText.fontStyle = FontStyles.Bold;
        defeatedMonstersText.raycastTarget = false;
        defeatedMonstersText.rectTransform.anchorMin = new Vector2(.5f, 1f);
        defeatedMonstersText.rectTransform.anchorMax = new Vector2(.5f, 1f);
        defeatedMonstersText.rectTransform.pivot = new Vector2(.5f, 1f);
        defeatedMonstersText.rectTransform.anchoredPosition = new Vector2(0f, -88f);
    }

    private void RefreshDefeatedMonstersCounter(int defeatedMonsters)
    {
        if (defeatedMonstersText != null)
            defeatedMonstersText.text = $"Monstruos derrotados: {defeatedMonsters}";
    }

    private void CreatePauseMenu()
    {
        RectTransform root = EnsureHudRoot();
        if (root == null || pauseMenu != null)
            return;

        pauseMenu = RuntimeUiFactory.Panel(root, "Gameplay Pause Menu", new Vector2(430f, 380f), Vector2.zero);
        pauseMenu.GetComponent<Image>().color = new Color(.025f, .075f, .085f, .96f);
        pauseMenuGroup = pauseMenu.gameObject.AddComponent<CanvasGroup>();
        RuntimeUiFactory.Text(pauseMenu, "PAUSA", new Vector2(0f, 125f), new Vector2(350f, 52f), 34).raycastTarget = false;
        RuntimeUiFactory.Text(pauseMenu, "La partida está detenida", new Vector2(0f, 75f), new Vector2(350f, 28f), 17).raycastTarget = false;
        RuntimeUiFactory.Button(pauseMenu, "SETTINGS", new Vector2(0f, 15f), new Vector2(285f, 54f), OpenSettingsFromPause);
        RuntimeUiFactory.Button(pauseMenu, "VOLVER AL MENÚ", new Vector2(0f, -52f), new Vector2(285f, 54f), ReturnToMainMenu);
        RuntimeUiFactory.Button(pauseMenu, "CONTINUAR", new Vector2(0f, -119f), new Vector2(285f, 54f), ResumeGameplay);
        pauseMenu.gameObject.SetActive(false);
    }

    private void OpenPauseMenu()
    {
        if (manager == null || !manager.IsRunning)
            return;

        manager.PauseGameplay();
        ShowPauseMenu();
    }

    public void ReturnToPauseMenu()
    {
        if (manager == null || !manager.IsRunning)
            return;

        if (settingsShortcut != null)
            settingsShortcut.interactable = true;
        manager.PauseGameplay();
        ShowPauseMenu();
    }

    private void ShowPauseMenu()
    {
        if (pauseMenu == null)
            CreatePauseMenu();
        if (pauseMenu == null || pauseMenuGroup == null)
            return;

        pauseMenu.DOKill();
        pauseMenuGroup.DOKill();
        pauseMenu.gameObject.SetActive(true);
        pauseMenuGroup.alpha = 0f;
        pauseMenuGroup.interactable = true;
        pauseMenuGroup.blocksRaycasts = true;
        pauseMenu.localScale = Vector3.one * .88f;
        pauseMenu.DOScale(Vector3.one, .18f).SetEase(Ease.OutBack);
        pauseMenuGroup.DOFade(1f, .16f);
    }

    private void HidePauseMenu(bool instant)
    {
        if (pauseMenu == null || !pauseMenu.gameObject.activeSelf)
            return;

        pauseMenu.DOKill();
        pauseMenuGroup.DOKill();
        pauseMenuGroup.interactable = false;
        pauseMenuGroup.blocksRaycasts = false;
        if (instant)
        {
            pauseMenu.gameObject.SetActive(false);
            return;
        }

        pauseMenu.DOScale(Vector3.one * .88f, .14f).SetEase(Ease.InBack).OnComplete(() => pauseMenu.gameObject.SetActive(false));
        pauseMenuGroup.DOFade(0f, .14f);
    }

    private void ResumeGameplay()
    {
        HidePauseMenu(false);
        if (settingsShortcut != null)
            settingsShortcut.interactable = true;
        manager?.ResumeGameplay();
    }

    private void OpenSettingsFromPause()
    {
        if (UiManager.Instance == null)
            return;

        SettingsUi settings = UiManager.Instance.GetWindow("SettingsUi")?.GetComponent<SettingsUi>();
        if (settings == null)
            return;

        settings.ConfigureReturnToPauseMenu();
        if (settingsShortcut != null)
            settingsShortcut.interactable = false;
        HidePauseMenu(true);
        UiManager.Instance.ShowWindow("SettingsUi");
    }

    private void ReturnToMainMenu()
    {
        HidePauseMenu(true);
        manager?.ReturnToMainMenu();
    }

    private void OnDestroy()
    {
        if (manager != null)
            manager.MonstersDefeatedChanged -= RefreshDefeatedMonstersCounter;
    }

    private void Update()
    {
        if (retreatButton != null && manager != null)
            retreatButton.gameObject.SetActive(manager.IsRunning && manager.RemainingTime <= 30f && manager.RemainingTime > 0f);
    }

    private void OpenSettings() => OpenPauseMenu();


}
