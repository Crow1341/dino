using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    [Header("Timer Settings")]
    [Tooltip("Tiempo limite en segundos (300 = 5 minutos)")]
    [SerializeField] private float timeLimit = 300f;

    [Header("UI (opcional)")]
    [SerializeField] private TMP_Text timerText;
    [Tooltip("El texto del tiempo aparece cuando queden estos segundos o menos (60 = ultimo minuto)")]
    [SerializeField] private float showTimerBelowSeconds = 60f;

    private float currentTime;
    private bool timerRunning = false;

    private GameplayManager gameplayManager;

    private void Start()
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

        // El texto solo se ve cuando queda poco tiempo
        bool show = currentTime <= showTimerBelowSeconds;
        if (timerText.gameObject.activeSelf != show)
            timerText.gameObject.SetActive(show);

        int total = Mathf.CeilToInt(currentTime);
        timerText.text = $"{total / 60:00}:{total % 60:00}";
    }
}
