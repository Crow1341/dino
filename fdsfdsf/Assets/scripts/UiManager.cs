using UnityEngine;
using System.Collections.Generic;
using System;

public class UiManager : MonoBehaviour

{
   public static UiManager Instance {get; private set;}

    [SerializeField] private List<UIWindow> _uiWindows;
    public List<UIWindow> UIWindows => _uiWindows;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Asegura que el UIManager no se destruya al cargar una nueva escena.
        DontDestroyOnLoad(this.gameObject);
    }

    private System.Collections.IEnumerator Start()
    {
        if (_uiWindows == null) _uiWindows = new List<UIWindow>();
        _uiWindows.RemoveAll(IsRetiredGameplayWindow);
        foreach (var discovered in FindObjectsByType<UIWindow>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (discovered == null) continue;
            if (IsRetiredGameplayWindow(discovered))
            {
                discovered.gameObject.SetActive(false);
                continue;
            }
            if (string.IsNullOrEmpty(discovered.Id))
                discovered.ConfigureIdentity(discovered is PopUi ? "PopUi" : discovered.GetType().Name, discovered.CanvasRectTransform != null ? discovered.CanvasRectTransform : discovered.transform as RectTransform);
            if (!_uiWindows.Contains(discovered)) _uiWindows.Add(discovered);
        }
        foreach (var button in FindObjectsByType<UnityEngine.UI.Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (button.GetComponent<ButtonFeedback>() == null) button.gameObject.AddComponent<ButtonFeedback>();
            var colors = button.colors;
            colors.normalColor = new Color(.48f, .31f, .16f, 1f);
            colors.highlightedColor = new Color(.76f, .55f, .27f, 1f);
            colors.pressedColor = new Color(.28f, .18f, .10f, 1f);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = new Color(.24f, .22f, .20f, .55f);
            colors.fadeDuration = .12f;
            button.colors = colors;
        }
        CreateRuntimeSettingsWindow();
        yield return null;
        if (FindWindow("StartUi") != null) ShowWindow("StartUi");
    }

    private void CreateRuntimeSettingsWindow()
    {
        if (FindWindow("SettingsUi") != null) return;
        UIWindow gameplay = FindWindow("GameplayUi");
        if (gameplay == null || gameplay.CanvasRectTransform == null) return;
        var go = new GameObject("SettingsUI (runtime)", typeof(RectTransform), typeof(CanvasGroup), typeof(SettingsUi));
        var rect = (RectTransform)go.transform; rect.SetParent(gameplay.CanvasRectTransform, false);
        rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero;
        var settings = go.GetComponent<SettingsUi>(); settings.ConfigureIdentity("SettingsUi", rect);
        _uiWindows.Add(settings); settings.Hide(true);
    }

    public void ShowWindow(string windowName)
    {
        UIWindow window = FindWindow(windowName);
        if (window == null) { Debug.LogError("Window not found: " + windowName); return; }
        Debug.Log("Showing window: " + window.Id);
        window.Show();
    }

    public void HideWindow(string windowName)
    {
        UIWindow window = FindWindow(windowName);
        if (window == null) { Debug.LogError("Window not found: " + windowName); return; }
        Debug.Log("Hiding window: " + window.Id);
        window.Hide();
    }

    public UIWindow GetWindow(string windowName)
    {
        UIWindow window = FindWindow(windowName);
        if (window == null) Debug.LogError("Window not found: " + windowName);
        return window;
    }

    private UIWindow FindWindow(string requestedId)
    {
        if (_uiWindows == null) return null;
        string requested = Normalize(requestedId);
        foreach (var window in _uiWindows)
        {
            if (window == null) continue;
            string id = Normalize(window.Id);
            if (id == requested || AliasMatches(requested, id)) return window;
        }
        return null;
    }

    private static string Normalize(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return value.Replace(" ", string.Empty).Replace("_", string.Empty).ToLowerInvariant();
    }

    private static bool AliasMatches(string requested, string id)
    {
        return (requested == "derrota" || requested == "defeat" || requested == "popui") && id == "popui";
    }

    private static bool IsRetiredGameplayWindow(UIWindow window)
    {
        return window is BackpackUi || window is GabilitiesUi;
    }
}
