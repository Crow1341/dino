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

    [Header("Nombres de las ventanas (deben ser iguales al Id de cada ventana)")]
    [SerializeField] private string Mochila = "Mochila";
    [SerializeField] private string Habilidades = "Habilidades";

    public override void Initialize()
    {
        base.Initialize();

        if (mochilaButton != null)
            mochilaButton.onClick.AddListener(() => OpenWindow(Mochila));
        else
            Debug.LogWarning("GameplayUi: falta arrastrar el boton de Mochila.");

        if (habilidadesButton != null)
            habilidadesButton.onClick.AddListener(() => OpenWindow(Habilidades));
        else
            Debug.LogWarning("GameplayUi: falta arrastrar el boton de Habilidades.");
    }

    // Abre la ventana ENCIMA del gameplay, sin cerrarlo
    private void OpenWindow(string windowName)
    {
        Debug.Log($"GameplayUi: abriendo la ventana '{windowName}'");
        UiManager.Instance.ShowWindow(windowName);
    }
    #region Test Methods

    [Button("Test Show")]
    private void TestShow()
    {
        Show();
    }

    [Button("Test Hide")]
    private void TestHide()
    {
        Hide();
    }

    #endregion
}
