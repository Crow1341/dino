using System;
using UnityEngine;

/// <summary>Permanent profile and temporary run wallet used by combat and shop systems.</summary>
public sealed class PlayerProgression : MonoBehaviour
{
    public const int SkillCount = 8;
    public const int MaxSkillRank = 3;
    private const int CurrentSkillSystemVersion = 2;
    private static readonly string[] SkillNames =
    {
        "Golpe afilado", "Hoja pesada", "Concentracion critica", "Cazador experto",
        "Iman de monedas", "Recolector", "Segundo aliento", "Amuleto de fortuna"
    };
    private static readonly string[] SkillDescriptions =
    {
        "+1 de dano por rango", "+2 de dano por rango", "+10% de probabilidad critica por rango", "+15% de XP por rango",
        "+15% de monedas por rango", "+1 material por rango", "El tiempo avanza 8% mas lento por rango", "+3% de probabilidad de evento por rango"
    };

    public static PlayerProgression Instance { get; private set; }
    public event Action Changed;
    public int PermanentXp { get; private set; }
    public int PermanentCoins { get; private set; }
    public int PermanentMaterials { get; private set; }
    public int RunXp { get; private set; }
    public int RunCoins { get; private set; }
    public int RunMaterials { get; private set; }
    public int Potions { get; private set; }
    public bool IsRunActive { get; private set; }
    public int RunEnemyKills { get; private set; }
    public float SavedEnemyHealth { get; private set; }
    public float SavedTimeRemaining { get; private set; }
    public int SavedSpriteIndex { get; private set; }
    public int EquippedSword { get; private set; }
    public float SwordDamage => EquippedSword == 1 ? 3f : EquippedSword == 2 ? 8f : 0f;
    public string EquippedSwordName => EquippedSword == 1 ? "Espada de acero" : EquippedSword == 2 ? "Espada encantada" : "Espada inicial";
    public string OwnedSwordSummary => "Inicial" + (HasSword(1) ? ", Acero" : string.Empty) + (HasSword(2) ? ", Encantada" : string.Empty);
    public int TotalXp => PermanentXp + RunXp;
    public int TotalCoins => PermanentCoins + RunCoins;
    public int TotalMaterials => PermanentMaterials + RunMaterials;
    public float XpRewardMultiplier => 1f + GetSkillLevel(3) * .15f;
    public float CoinRewardMultiplier => 1f + GetSkillLevel(4) * .15f;
    public int BonusMaterialsPerEnemy => GetSkillLevel(5);
    public float TimerDrainMultiplier => Mathf.Max(.2f, 1f - GetSkillLevel(6) * .08f);
    public float SpecialEventChanceBonus => GetSkillLevel(7) * .03f;

    public static PlayerProgression EnsureInstance()
    {
        if (Instance != null) return Instance;
        var go = new GameObject("PlayerProgression");
        return go.AddComponent<PlayerProgression>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    public void AddRunRewards(int xp, int coins, int materials)
    {
        RunXp += Mathf.Max(0, xp); RunCoins += Mathf.Max(0, coins); RunMaterials += Mathf.Max(0, materials); Save(); Changed?.Invoke();
    }

    public void BeginNewRun()
    {
        RunXp = RunCoins = RunMaterials = 0; RunEnemyKills = 0; SavedEnemyHealth = 0; SavedTimeRemaining = 300f; SavedSpriteIndex = -1; IsRunActive = true; Save(); Changed?.Invoke();
    }

    public void SaveRunSnapshot(float timeRemaining, int kills, float enemyHealth, int spriteIndex)
    {
        if (!IsRunActive) return;
        SavedTimeRemaining = Mathf.Max(0, timeRemaining); RunEnemyKills = Mathf.Max(0, kills); SavedEnemyHealth = Mathf.Max(0, enemyHealth); SavedSpriteIndex = spriteIndex; Save();
    }

    public bool SpendCoins(int amount)
    {
        if (amount < 0 || TotalCoins < amount) return false;
        int fromRun = Mathf.Min(RunCoins, amount); RunCoins -= fromRun; PermanentCoins -= amount - fromRun;
        Save(); Changed?.Invoke(); return true;
    }

    public bool SpendXp(int amount)
    {
        if (amount < 0 || TotalXp < amount) return false;
        int fromRun = Mathf.Min(RunXp, amount); RunXp -= fromRun; PermanentXp -= amount - fromRun;
        Save(); Changed?.Invoke(); return true;
    }

    public bool SpendMaterials(int amount)
    {
        if (amount < 0 || TotalMaterials < amount) return false;
        int fromRun = Mathf.Min(RunMaterials, amount); RunMaterials -= fromRun; PermanentMaterials -= amount - fromRun;
        Save(); Changed?.Invoke(); return true;
    }

    public void AddPotion() { Potions++; Save(); Changed?.Invoke(); }
    public bool UsePotion() { if (Potions <= 0) return false; Potions--; Save(); Changed?.Invoke(); return true; }

    public static string GetSkillName(int skill) => IsValidSkill(skill) ? SkillNames[skill] : string.Empty;
    public static string GetSkillDescription(int skill) => IsValidSkill(skill) ? SkillDescriptions[skill] : string.Empty;

    public int GetSkillLevel(int skill) => IsValidSkill(skill) ? PlayerPrefs.GetInt("Skill." + skill, 0) : 0;
    public int GetSkillCost(int skill) => IsValidSkill(skill) ? 20 * (GetSkillLevel(skill) + 1) * (skill + 1) : 0;
    public bool UpgradeSkill(int skill)
    {
        if (!IsValidSkill(skill) || GetSkillLevel(skill) >= MaxSkillRank) return false;
        int cost = GetSkillCost(skill);
        if (!SpendXp(cost)) return false;
        PlayerPrefs.SetInt("Skill." + skill, GetSkillLevel(skill) + 1); PlayerPrefs.Save(); Changed?.Invoke(); return true;
    }

    public bool CraftSword(int sword) { return CraftSword(sword, sword == 1 ? 3 : 8); }

    public bool CraftSword(int sword, int cost)
    {
        if (sword < 1 || sword > 2 || PlayerPrefs.GetInt("Sword." + sword, 0) != 0) return false;
        if (!SpendMaterials(cost)) return false;
        PlayerPrefs.SetInt("Sword." + sword, 1); PlayerPrefs.Save(); Changed?.Invoke(); return true;
    }

    public bool HasSword(int sword) => sword == 0 || PlayerPrefs.GetInt("Sword." + sword, 0) != 0;
    public void EquipSword(int sword) { if (!HasSword(sword)) return; EquippedSword = sword; PlayerPrefs.SetInt("EquippedSword", sword); PlayerPrefs.Save(); Changed?.Invoke(); }

    public void RetireRun()
    {
        PermanentXp += RunXp; PermanentCoins += RunCoins; PermanentMaterials += RunMaterials;
        RunXp = RunCoins = RunMaterials = 0; IsRunActive = false; RunEnemyKills = 0; SavedEnemyHealth = 0; SavedTimeRemaining = 0; SavedSpriteIndex = -1; Save(); Changed?.Invoke();
    }

    public void LoseRun()
    {
        RunXp = RunCoins = RunMaterials = 0; IsRunActive = false; RunEnemyKills = 0; SavedEnemyHealth = 0; SavedTimeRemaining = 0; SavedSpriteIndex = -1; Save(); Changed?.Invoke();
    }

    private void Save()
    {
        PlayerPrefs.SetInt("Profile.Xp", PermanentXp); PlayerPrefs.SetInt("Profile.Coins", PermanentCoins);
        PlayerPrefs.SetInt("Profile.Materials", PermanentMaterials); PlayerPrefs.SetInt("Profile.Potions", Potions);
        PlayerPrefs.SetInt("Run.Xp", RunXp); PlayerPrefs.SetInt("Run.Coins", RunCoins); PlayerPrefs.SetInt("Run.Materials", RunMaterials);
        PlayerPrefs.SetInt("Run.Active", IsRunActive ? 1 : 0); PlayerPrefs.SetInt("Run.Kills", RunEnemyKills);
        PlayerPrefs.SetFloat("Run.EnemyHealth", SavedEnemyHealth); PlayerPrefs.SetFloat("Run.Time", SavedTimeRemaining); PlayerPrefs.SetInt("Run.Sprite", SavedSpriteIndex); PlayerPrefs.Save();
    }

    private void Load()
    {
        PermanentXp = PlayerPrefs.GetInt("Profile.Xp", 0); PermanentCoins = PlayerPrefs.GetInt("Profile.Coins", 0);
        PermanentMaterials = PlayerPrefs.GetInt("Profile.Materials", 0); Potions = PlayerPrefs.GetInt("Profile.Potions", 0);
        EquippedSword = PlayerPrefs.GetInt("EquippedSword", 0);
        RunXp = PlayerPrefs.GetInt("Run.Xp", 0); RunCoins = PlayerPrefs.GetInt("Run.Coins", 0); RunMaterials = PlayerPrefs.GetInt("Run.Materials", 0);
        IsRunActive = PlayerPrefs.GetInt("Run.Active", 0) == 1; RunEnemyKills = PlayerPrefs.GetInt("Run.Kills", 0);
        SavedEnemyHealth = PlayerPrefs.GetFloat("Run.EnemyHealth", 0); SavedTimeRemaining = PlayerPrefs.GetFloat("Run.Time", 300f);
        SavedSpriteIndex = PlayerPrefs.GetInt("Run.Sprite", -1);
        MigrateSkillData();
    }

    private static bool IsValidSkill(int skill) => skill >= 0 && skill < SkillCount;

    private static void MigrateSkillData()
    {
        int savedVersion = PlayerPrefs.GetInt("Skill.SystemVersion", 0);
        if (savedVersion >= CurrentSkillSystemVersion)
            return;

        for (int skill = 0; skill < SkillCount; skill++)
        {
            string key = "Skill." + skill;
            if (PlayerPrefs.HasKey(key))
                PlayerPrefs.SetInt(key, Mathf.Clamp(PlayerPrefs.GetInt(key), 0, MaxSkillRank));
        }

        PlayerPrefs.SetInt("Skill.SystemVersion", CurrentSkillSystemVersion);
        PlayerPrefs.Save();
    }
}
