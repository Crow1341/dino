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

    private void Awake()
    {
        gameplayManager = FindAnyObjectByType<GameplayManager>();
        currentTime = timeLimit;
        RefreshUI();
    }

    public void StartTimer()
    {
        currentTime = timeLimit;
        timerRunning = true;
        RefreshUI();
    }

    private void Update()
    {
        if (!timerRunning)
            return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            timerRunning = false;
            RefreshUI();

            gameplayManager.Defeat();
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
