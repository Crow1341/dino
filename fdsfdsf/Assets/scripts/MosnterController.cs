using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MosnterController : MonoBehaviour
{

    [Header("Monster Stats")]
    [SerializeField] private float baseMaxHealth = 1000f;
    [Tooltip("La vida se multiplica por esto con cada monstruo derrotado (1 = siempre igual)")]
    [SerializeField] private float healthMultiplierPerKill = 1.1f;

    [Header("Sprites (aparece uno al azar cuando muere el anterior)")]
    [SerializeField] private Sprite[] monsterSprites;

    [Header("Visual del monstruo (usa uno)")]
    [SerializeField] private Image monsterImage;
    [SerializeField] private SpriteRenderer monsterSpriteRenderer;

    [Header("UI (opcional)")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Slider healthBar;

    private float currentHealth;
    private float maxHealth;
    private int killCount = 0;
    private int lastSpriteIndex = -1;
    private GameplayManager gameplayManager;

    private void Awake()
    {
        gameplayManager = FindFirstObjectByType<GameplayManager>();
        LoadMonster();
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0)
            return;

        currentHealth -= damage;
        Debug.Log($"Vida del monstruo: {Mathf.Max(0f, currentHealth)} / {maxHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            RefreshUI();
            gameplayManager.MonsterDefeated();
            return;
        }

        RefreshUI();
    }

    // Reinicia la partida: vida base y un monstruo al azar
    public void ResetMonster()
    {
        killCount = 0;
        LoadMonster();
    }

    // Aparece un monstruo nuevo al azar
    public void NextMonster()
    {
        killCount++;
        LoadMonster();
    }

    private void LoadMonster()
    {
        maxHealth = baseMaxHealth * Mathf.Pow(healthMultiplierPerKill, killCount);
        currentHealth = maxHealth;

        SetRandomSprite();
        RefreshUI();
    }

    private void SetRandomSprite()
    {
        if (monsterSprites == null || monsterSprites.Length == 0)
            return;

        int index = Random.Range(0, monsterSprites.Length);

        // Evita que salga el mismo monstruo dos veces seguidas
        if (monsterSprites.Length > 1)
        {
            while (index == lastSpriteIndex)
                index = Random.Range(0, monsterSprites.Length);
        }

        lastSpriteIndex = index;
        Sprite sprite = monsterSprites[index];

        if (monsterImage != null)
            monsterImage.sprite = sprite;

        if (monsterSpriteRenderer != null)
            monsterSpriteRenderer.sprite = sprite;
    }

    private void RefreshUI()
    {
        if (healthText != null)
            healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";

        if (healthBar != null)
        {
            healthBar.minValue = 0f;
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }
}
