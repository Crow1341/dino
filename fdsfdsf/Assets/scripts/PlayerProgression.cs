using System;
using UnityEngine;

/// <summary>Permanent profile and temporary run wallet used by combat and shop systems.</summary>
public sealed class PlayerProgression : MonoBehaviour
{
    public static PlayerProgression Instance { get; private set; }
    public event Action Changed;
    public int PermanentXp { get; private set; }
    public int PermanentCoins { get; private set; }
    public int PermanentMaterials { get; private set; }
    public int RunXp { get; private set; }
    public int RunCoins { get; private set; }
    public int RunMaterials { get; private set; }
    public int Potions { get; private set; }
    public int EquippedSword { get; private set; }
    public float SwordDamage => EquippedSword == 1 ? 3f : EquippedSword == 2 ? 8f : 0f;
    public int TotalXp => PermanentXp + RunXp;
    public int TotalCoins => PermanentCoins + RunCoins;
    public int TotalMaterials => PermanentMaterials + RunMaterials;

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
        RunXp += Mathf.Max(0, xp); RunCoins += Mathf.Max(0, coins); RunMaterials += Mathf.Max(0, materials); Changed?.Invoke();
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

    public void AddPotion() { Potions++; Changed?.Invoke(); }
    public bool UsePotion() { if (Potions <= 0) return false; Potions--; Changed?.Invoke(); return true; }

    public int GetSkillLevel(int skill) => PlayerPrefs.GetInt("Skill." + skill, 0);
    public int GetSkillCost(int skill) => 20 * (GetSkillLevel(skill) + 1) * (skill + 1);
    public bool UpgradeSkill(int skill)
    {
        if (skill < 0 || skill > 2 || GetSkillLevel(skill) >= 3) return false;
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
        RunXp = RunCoins = RunMaterials = 0; Save(); Changed?.Invoke();
    }

    public void LoseRun()
    {
        RunXp = RunCoins = RunMaterials = 0; Save(); Changed?.Invoke();
    }

    private void Save()
    {
        PlayerPrefs.SetInt("Profile.Xp", PermanentXp); PlayerPrefs.SetInt("Profile.Coins", PermanentCoins);
        PlayerPrefs.SetInt("Profile.Materials", PermanentMaterials); PlayerPrefs.SetInt("Profile.Potions", Potions); PlayerPrefs.Save();
    }

    private void Load()
    {
        PermanentXp = PlayerPrefs.GetInt("Profile.Xp", 0); PermanentCoins = PlayerPrefs.GetInt("Profile.Coins", 0);
        PermanentMaterials = PlayerPrefs.GetInt("Profile.Materials", 0); Potions = PlayerPrefs.GetInt("Profile.Potions", 0);
        EquippedSword = PlayerPrefs.GetInt("EquippedSword", 0);
    }
}
