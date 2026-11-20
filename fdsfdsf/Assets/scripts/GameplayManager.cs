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

    [Header("Run rewards (Inspector)")]
    [SerializeField] private Vector2Int xpPerEnemy = new Vector2Int(5, 10);
    [SerializeField] private Vector2Int coinsPerEnemy = new Vector2Int(2, 6);
    [SerializeField] private Vector2Int materialsPerEnemy = new Vector2Int(0, 2);
    [SerializeField, Range(0f, 1f)] private float witchChance = 0.05f;
    [SerializeField, Range(0f, 1f)] private float blacksmithChance = 0.05f;

    private bool gameEnded = true;
    private bool defeatShown;
    private float potionBonus;
    private Coroutine potionRoutine;
    private readonly List<RaycastResult> raycastResults = new List<RaycastResult>();

    // Se ejecuta cada vez que la UI de Gamplay se activa
    private void OnEnable()
    {
        SpecialEventRequested += HandleSpecialEvent;
        ignoreClicksOverButtons = true;
        if (startOnPlay)
            StartGame();
    }

    // Al desactivarse la UI, la partida se pausa
    private void OnDisable()
    {
        SpecialEventRequested -= HandleSpecialEvent;
        gameEnded = true;

        if (timer != null)
            timer.StopTimer();
    }

    private void Update()
    {
        if (gameEnded)
            return;

        if (ClickedThisFrame() && monster != null)
        {
            float skillBonus = PlayerProgression.Instance != null ? PlayerProgression.Instance.GetSkillLevel(0) : 0;
            if (PlayerProgression.Instance != null) skillBonus += PlayerProgression.Instance.GetSkillLevel(1) * 2f;
            float swordBonus = PlayerProgression.Instance != null ? PlayerProgression.Instance.SwordDamage : 0;
            float damage = damagePerClick + skillBonus + swordBonus + potionBonus;
            if (PlayerProgression.Instance != null && Random.value < PlayerProgression.Instance.GetSkillLevel(2) * 0.1f) damage *= 2f;
            monster.TakeDamage(damage);
        }
    }

    public void StartGame()
    {
        if (monster == null || timer == null) { Debug.LogError("GameplayManager requires Monster and Timer references.", this); return; }
        PlayerProgression.EnsureInstance();
        gameEnded = false;
        defeatShown = false;
        monster.ResetMonster();
        timer.StartTimer();
    }

    public void MonsterDefeated()
    {
        if (gameEnded)
            return;

        GrantEnemyRewards();
        monster.NextMonster();
        timer.StartTimer();

        Debug.Log("Next monster appeared!");
    }

    public void Defeat()
    {
        if (gameEnded || defeatShown)
            return;

        gameEnded = true;
        defeatShown = true;
        timer.StopTimer();
        if (monster != null) monster.StopCombat();
        if (PlayerProgression.Instance != null) PlayerProgression.Instance.LoseRun();
        if (UiManager.Instance != null) UiManager.Instance.ShowWindow(defeatWindow);
        else Debug.LogError("UiManager is missing; cannot show defeat window.", this);

        Debug.Log("Game Over!");
    }

    // Para mejoras desde otros scripts
    public void AddClickDamage(float amount)
    {
        damagePerClick = Mathf.Max(0f, damagePerClick + amount);
    }

    public bool IsRunning => !gameEnded;
    public float DamagePerClick => damagePerClick;
    public float RemainingTime => timer != null ? timer.RemainingTime : 0f;

    public void UseDamagePotion()
    {
        if (gameEnded || PlayerProgression.Instance == null || !PlayerProgression.Instance.UsePotion()) return;
        if (potionRoutine != null) StopCoroutine(potionRoutine);
        potionBonus = 5f;
        potionRoutine = StartCoroutine(PotionBoost());
    }

    private System.Collections.IEnumerator PotionBoost()
    {
        yield return new WaitForSeconds(20f);
        potionBonus = 0f; potionRoutine = null;
    }

    public void Retreat()
    {
        if (gameEnded) return;
        gameEnded = true; timer.StopTimer();
        if (monster != null) monster.StopCombat();
        if (PlayerProgression.Instance != null) PlayerProgression.Instance.RetireRun();
        if (UiManager.Instance != null) UiManager.Instance.HideWindow("GameplayUi");
        if (UiManager.Instance != null) UiManager.Instance.ShowWindow("StartUi");
    }

    public void RestartGame()
    {
        if (UiManager.Instance != null) UiManager.Instance.HideWindow(defeatWindow);
        if (UiManager.Instance != null) UiManager.Instance.ShowWindow("GameplayUi");
        StartGame();
    }

    private void GrantEnemyRewards()
    {
        PlayerProgression progress = PlayerProgression.Instance;
        if (progress == null) return;
        int xp = Random.Range(Mathf.Min(xpPerEnemy.x, xpPerEnemy.y), Mathf.Max(xpPerEnemy.x, xpPerEnemy.y) + 1);
        int coins = Random.Range(Mathf.Min(coinsPerEnemy.x, coinsPerEnemy.y), Mathf.Max(coinsPerEnemy.x, coinsPerEnemy.y) + 1);
        int materials = Random.Range(Mathf.Min(materialsPerEnemy.x, materialsPerEnemy.y), Mathf.Max(materialsPerEnemy.x, materialsPerEnemy.y) + 1);
        progress.AddRunRewards(xp, coins, materials);
        Debug.Log($"Enemy reward: +{xp} XP, +{coins} coins, +{materials} materials");
        float eventRoll = Random.value;
        if (eventRoll < witchChance) SpecialEventRequested?.Invoke(SpecialEvent.Witch);
        else if (eventRoll < witchChance + blacksmithChance) SpecialEventRequested?.Invoke(SpecialEvent.Blacksmith);
    }

    public enum SpecialEvent { Witch, Blacksmith }
    public event System.Action<SpecialEvent> SpecialEventRequested;

    private void HandleSpecialEvent(SpecialEvent specialEvent)
    {
        if (UiManager.Instance == null) return;
        UiManager.Instance.ShowWindow(specialEvent == SpecialEvent.Witch ? "WitchUi" : "BlacksmithUi");
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
