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
        foreach (var discovered in FindObjectsByType<UIWindow>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (discovered == null) continue;
            if (string.IsNullOrEmpty(discovered.Id))
                discovered.ConfigureIdentity(discovered is PopUi ? "PopUi" : discovered.GetType().Name, discovered.CanvasRectTransform != null ? discovered.CanvasRectTransform : discovered.transform as RectTransform);
            if (!_uiWindows.Contains(discovered)) _uiWindows.Add(discovered);
        }
        CreateRuntimeSettingsWindow();
        CreateRuntimeEventWindow("WitchUi", true);
        CreateRuntimeEventWindow("BlacksmithUi", false);
        yield return null;
        if (GetWindow("StartUi") != null) ShowWindow("StartUi");
    }

    private void CreateRuntimeSettingsWindow()
    {
        if (GetWindow("SettingsUi") != null) return;
        UIWindow gameplay = GetWindow("GameplayUi");
        if (gameplay == null || gameplay.CanvasRectTransform == null) return;
        var go = new GameObject("SettingsUI (runtime)", typeof(RectTransform), typeof(CanvasGroup), typeof(SettingsUi));
        var rect = (RectTransform)go.transform; rect.SetParent(gameplay.CanvasRectTransform, false);
        rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero;
        var settings = go.GetComponent<SettingsUi>(); settings.ConfigureIdentity("SettingsUi", rect);
        _uiWindows.Add(settings); settings.Hide(true);
    }

    private void CreateRuntimeEventWindow(string id, bool witch)
    {
        if (GetWindow(id) != null) return;
        UIWindow gameplay = GetWindow("GameplayUi");
        if (gameplay == null || gameplay.CanvasRectTransform == null) return;
        var go = new GameObject(id + " (runtime)", typeof(RectTransform), typeof(CanvasGroup), typeof(EventShopUi));
        var rect = (RectTransform)go.transform; rect.SetParent(gameplay.CanvasRectTransform, false);
        rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero;
        var shop = go.GetComponent<EventShopUi>(); shop.ConfigureIdentity(id, rect); shop.SetWitch(witch);
        _uiWindows.Add(shop); shop.Hide(true);
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
        return (requested == "mochila" || requested == "backpackui" || requested == "backpakcui") && id == "backpakcui"
            || (requested == "habilidades" || requested == "habilitiesui") && id == "gabilitiesui"
            || (requested == "derrota" || requested == "defeat" || requested == "popui") && id == "popui";
    }
}
