using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class GameplayManager : MonoBehaviour
{
    [Header("Monster")]
    [SerializeField] private MosnterController monster;

    [Header("Timer")]
    [SerializeField] private TimeController timer;

    [Header("Click Damage")]
    [SerializeField] private float damagePerClick = 1f;
    [Tooltip("Si esta activo, los clics sobre botones NO hacen daño (el resto de la pantalla si)")]
    [SerializeField] private bool ignoreClicksOverButtons = true;

    [Header("Windows")]
    [Tooltip("Nombre de la ventana que se abre al acabarse el tiempo (el mismo con el que la registras en UIManager)")]
    [SerializeField] private string defeatWindow = "popui";

    [Header("Start")]
    [Tooltip("Inicia la partida automaticamente cada vez que se activa la UI de Gamplay")]
    [SerializeField] private bool startOnPlay = true;

    private bool gameEnded = true;
    private readonly List<RaycastResult> raycastResults = new List<RaycastResult>();

    // Se ejecuta cada vez que la UI de Gamplay se activa
    private void OnEnable()
    {
        if (startOnPlay)
            StartGame();
    }

    // Al desactivarse la UI, la partida se pausa
    private void OnDisable()
    {
        gameEnded = true;

        if (timer != null)
            timer.StopTimer();
    }

    private void Update()
    {
        if (gameEnded)
            return;

        if (ClickedThisFrame())
            monster.TakeDamage(damagePerClick);
    }

    public void StartGame()
    {
        gameEnded = false;
        monster.ResetMonster();
        timer.StartTimer();
    }

    public void MonsterDefeated()
    {
        if (gameEnded)
            return;

        monster.NextMonster();
        timer.StartTimer();

        Debug.Log("Next monster appeared!");
    }

    public void Defeat()
    {
        if (gameEnded)
            return;

        gameEnded = true;
        timer.StopTimer();

        UiManager.Instance.ShowWindow(defeatWindow);

        Debug.Log("Game Over!");
    }

    // Para mejoras desde otros scripts
    public void AddClickDamage(float amount)
    {
        damagePerClick += amount;
    }

    private bool ClickedThisFrame()
    {
        bool pressed;
        Vector2 position;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        if (Pointer.current == null)
            return false;
 
        pressed = Pointer.current.press.wasPressedThisFrame;
        position = Pointer.current.position.ReadValue();
#else
        bool touched = Input.touchCount > 0;
        pressed = Input.GetMouseButtonDown(0) ||
                  (touched && Input.GetTouch(0).phase == TouchPhase.Began);
        position = touched ? (Vector2)Input.GetTouch(0).position : (Vector2)Input.mousePosition;
#endif

        if (!pressed)
            return false;

        return !(ignoreClicksOverButtons && IsOverButton(position));
    }

    // True solo si bajo el puntero hay un boton (u otro elemento interactivo)
    private bool IsOverButton(Vector2 screenPosition)
    {
        if (EventSystem.current == null)
            return false;

        PointerEventData data = new PointerEventData(EventSystem.current) { position = screenPosition };
        raycastResults.Clear();
        EventSystem.current.RaycastAll(data, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject.GetComponentInParent<Selectable>() != null)
                return true;
        }

        return false;
    }
}
