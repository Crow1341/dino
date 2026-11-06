using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

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
        foreach (var window in _uiWindows)
        {
            if (window.Id == windowName)
            {
                Debug.Log("Showing window: " + windowName);
                window.Show();
                return;
            }
        }

        Debug.LogError("Window not found: " + windowName);
    }

    public void HideWindow(string windowName)
    {
        foreach (var window in _uiWindows)
        {
            if (window.Id == windowName)
            {
                Debug.Log("Hiding window: " + windowName);
                window.Hide();
                return;
            }
        }

        Debug.LogError("Window not found: " + windowName);
    }

    public UIWindow GetWindow(string windowName)
    {
        foreach (var window in _uiWindows)
        {
            if (window.Id == windowName)
            {
                return window;
            }
        }

        Debug.LogError("Window not found: " + windowName);
        return null;
    }
}
