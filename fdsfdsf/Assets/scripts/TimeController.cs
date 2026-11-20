using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    [Header("Timer Settings")]
    [Tooltip("Tiempo limite en segundos (300 = 5 minutos)")]
    [SerializeField] private float timeLimit = 300f;

    [Header("UI (opcional)")]
    [SerializeField] private TMP_Text timerText;
    [Tooltip("Umbral para activar el estilo de advertencia")]
    [SerializeField] private float warningBelowSeconds = 30f;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = new Color(1f, 0.25f, 0.2f);

    private float currentTime;
    private bool timerRunning = false;

    private GameplayManager gameplayManager;
    public float RemainingTime => currentTime;
    private int lastSavedSecond = -1;

    private void Awake()
    {
        gameplayManager = FindAnyObjectByType<GameplayManager>();
        if (timerText == null)
        {
            var parent = transform.parent as RectTransform;
            if (parent == null)
            {
                var canvas = FindAnyObjectByType<Canvas>();
                if (canvas != null) parent = canvas.transform as RectTransform;
            }
            if (parent != null)
            {
                timerText = RuntimeUiFactory.Text(parent, "05:00", new Vector2(0, -40), new Vector2(240, 70), 38);
                timerText.name = "Run Timer Text"; timerText.fontStyle = TMPro.FontStyles.Bold;
                timerText.rectTransform.anchorMin = timerText.rectTransform.anchorMax = new Vector2(.5f, 1f);
                timerText.rectTransform.pivot = new Vector2(.5f, 1f);
            }
        }
        currentTime = timeLimit;
        RefreshUI();
    }

    public void StartTimer()
    {
        StartTimer(timeLimit);
    }

    public void StartTimer(float seconds)
    {
        currentTime = Mathf.Clamp(seconds, 0f, timeLimit);
        timerRunning = true;
        lastSavedSecond = -1;
        RefreshUI();
    }

    private void Update()
    {
        if (!timerRunning)
            return;

        currentTime -= Time.deltaTime;
        int wholeSecond = Mathf.CeilToInt(currentTime);
        if (wholeSecond != lastSavedSecond)
        {
            lastSavedSecond = wholeSecond;
            if (PlayerProgression.Instance != null && gameplayManager != null && gameplayManager.IsRunning && gameplayManager.GetMonster() != null)
                PlayerProgression.Instance.SaveRunSnapshot(currentTime, gameplayManager.GetMonster().KillCount, gameplayManager.GetMonster().CurrentHealth, gameplayManager.GetMonster().CurrentSpriteIndex);
        }

        if (currentTime <= 0)
        {
            currentTime = 0;
            timerRunning = false;
            RefreshUI();

            if (gameplayManager != null) gameplayManager.Defeat();
            return;
        }

        RefreshUI();
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    private void RefreshUI()
    {
        if (timerText == null)
            return;

        if (!timerText.gameObject.activeSelf) timerText.gameObject.SetActive(true);

        int total = Mathf.CeilToInt(currentTime);
        timerText.text = $"{total / 60:00}:{total % 60:00}";
        timerText.color = currentTime <= warningBelowSeconds ? warningColor : normalColor;
    }
}
