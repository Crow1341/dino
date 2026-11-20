using TMPro;
using DG.Tweening;
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
    private bool defeated;
    private Transform animatedVisual;
    private Vector3 visualBaseScale = Vector3.one;
    public int KillCount => killCount;
    public float CurrentHealth => currentHealth;
    public int CurrentSpriteIndex => lastSpriteIndex;

    private void Awake()
    {
        gameplayManager = FindFirstObjectByType<GameplayManager>();
        animatedVisual = monsterImage != null ? monsterImage.transform : monsterSpriteRenderer != null ? monsterSpriteRenderer.transform : transform;
        visualBaseScale = animatedVisual.localScale;
        if (healthText != null) healthText.gameObject.SetActive(false);
        if (healthBar != null) healthBar.gameObject.SetActive(false);
        LoadMonster();
    }

    public void TakeDamage(float damage)
    {
        if (defeated || currentHealth <= 0 || damage <= 0f)
            return;

        currentHealth -= damage;
        PlayHitEffect();
        Debug.Log($"Vida del monstruo: {Mathf.Max(0f, currentHealth)} / {maxHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            RefreshUI();
            defeated = true;
            if (gameplayManager != null) StartCoroutine(DefeatAnimation());
            return;
        }

        RefreshUI();
    }

    // Reinicia la partida: vida base y un monstruo al azar
    public void ResetMonster()
    {
        killCount = 0;
        defeated = false;
        LoadMonster();
    }

    // Aparece un monstruo nuevo al azar
    public void NextMonster()
    {
        killCount++;
        defeated = false;
        LoadMonster();
    }

    public void StopCombat() { defeated = true; }

    private void PlayHitEffect()
    {
        if (animatedVisual == null) return;
        animatedVisual.DOKill(); animatedVisual.localScale = visualBaseScale;
        animatedVisual.DOScale(visualBaseScale * 0.94f, 0.07f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutQuad);
        if (monsterImage != null)
        {
            monsterImage.DOKill(); monsterImage.DOColor(new Color(1f, .55f, .48f), .06f).SetLoops(2, LoopType.Yoyo);
        }
    }

    private System.Collections.IEnumerator DefeatAnimation()
    {
        if (animatedVisual != null) { animatedVisual.DOKill(); animatedVisual.DOScale(Vector3.zero, .14f).SetEase(Ease.InBack); }
        yield return new WaitForSeconds(.15f);
        if (gameplayManager != null) gameplayManager.MonsterDefeated();
    }

    public void RestoreRun(int kills, float health, int spriteIndex)
    {
        killCount = Mathf.Max(0, kills); defeated = false; LoadMonster();
        if (spriteIndex >= 0 && monsterSprites != null && spriteIndex < monsterSprites.Length) SetSprite(spriteIndex);
        if (health > 0) currentHealth = Mathf.Min(maxHealth, health);
        RefreshUI();
    }

    private void LoadMonster()
    {
        maxHealth = baseMaxHealth * Mathf.Pow(healthMultiplierPerKill, killCount);
        currentHealth = maxHealth;
        if (nameText != null) nameText.text = $"Enemy {killCount + 1}";

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

        SetSprite(index);
    }

    private void SetSprite(int index)
    {
        if (monsterSprites == null || index < 0 || index >= monsterSprites.Length) return;
        lastSpriteIndex = index;
        Sprite sprite = monsterSprites[index];

        if (monsterImage != null)
            monsterImage.sprite = sprite;

        if (monsterSpriteRenderer != null)
            monsterSpriteRenderer.sprite = sprite;

        if (animatedVisual != null)
        {
            animatedVisual.DOKill(); animatedVisual.localScale = Vector3.zero;
            animatedVisual.DOScale(visualBaseScale, .32f).SetEase(Ease.OutBack);
        }
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
