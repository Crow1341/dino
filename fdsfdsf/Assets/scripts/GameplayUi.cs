using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;


public class GameplayUi : UIWindow
{
    [Header("Botones")]
    public Button mochilaButton;
    public Button habilidadesButton;
    private Button settingsShortcut;
    private Button retreatButton;
    private GameplayManager manager;

    [Header("Nombres de las ventanas")]
    [SerializeField] private string Mochila = "BackpackUi";
    [SerializeField] private string Habilidades = "GabilitiesUi";

    public override void Initialize()
    {
        base.Initialize();

        if (mochilaButton != null)
            mochilaButton.onClick.AddListener(() => OpenWindow(Mochila));
        else
            Debug.LogWarning("Falta asignar el boton de Mochila.");

        if (habilidadesButton != null)
            habilidadesButton.onClick.AddListener(() => OpenWindow(Habilidades));
        else
            Debug.LogWarning("Falta asignar el boton de Habilidades.");

        manager = FindFirstObjectByType<GameplayManager>();
        CreateShortcuts();
    }

    private void CreateShortcuts()
    {
        RectTransform root = CanvasRectTransform;
        if (root == null) return;
        settingsShortcut = RuntimeUiFactory.Button(root, "SETTINGS", new Vector2(-150, 280), new Vector2(190, 56), () => UiManager.Instance?.ShowWindow("SettingsUi"));
        retreatButton = RuntimeUiFactory.Button(root, "RETREAT", new Vector2(150, 280), new Vector2(190, 56), () => manager?.Retreat());
        retreatButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (retreatButton != null && manager != null)
            retreatButton.gameObject.SetActive(manager.IsRunning && manager.RemainingTime <= 30f && manager.RemainingTime > 0f);
    }

    private void OpenWindow(string windowName)
    {
       
        Debug.Log("Boton presionado. Ventana: " + windowName);
        UiManager.Instance.ShowWindow(windowName);
    }


}
