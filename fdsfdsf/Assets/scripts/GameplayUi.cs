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

    [Header("Nombres de las ventanas")]
    [SerializeField] private string Mochila = "BackpackUi";
    [SerializeField] private string Habilidades = "HabilitiesUi";

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
    }

    private void OpenWindow(string windowName)
    {
        Debug.Log("Intentando abrir: " + windowName);
        UiManager.Instance.ShowWindow(windowName);

        Debug.Log("Boton presionado. Ventana: " + windowName);
        UiManager.Instance.ShowWindow(windowName);
    }


}
