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
    [SerializeField] private string Mochila = "Mochila";
    [SerializeField] private string Habilidades = "Habilidades";

    public override void Initialize()
    {
        base.Initialize();

        if (mochilaButton != null)
        {
            mochilaButton.onClick.AddListener(() => OpenWindow(Mochila));
        }
        else
        {
            Debug.LogWarning("Falta asignar el boton de Mochila.");
        }

        if (habilidadesButton != null)
        {
            habilidadesButton.onClick.AddListener(() => OpenWindow(Habilidades));
        }
        else
        {
            Debug.LogWarning("Falta asignar el boton de Habilidades.");
        }
    }

    private void OpenWindow(string windowName)
    {
        Debug.Log("Abriendo ventana: " + windowName);
        UiManager.Instance.ShowWindow(windowName);
    }


}
