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
