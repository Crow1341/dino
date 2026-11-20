using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Runtime witch/blacksmith event panel, registered through the existing window manager.</summary>
public class EventShopUi : UIWindow
{
    [SerializeField] private bool witch;
    [SerializeField] private int potionPrice = 5;
    [SerializeField] private int steelSwordMaterials = 3;
    [SerializeField] private int enchantedSwordMaterials = 8;
    private TMP_Text status;
    private Button potionButton;
    private Button[] swordButtons;

    public void SetWitch(bool value) { witch = value; }

    public override void Initialize()
    {
        base.Initialize();
        if (CanvasRectTransform == null) return;
        var panel = RuntimeUiFactory.Panel(CanvasRectTransform, witch ? "Witch Event" : "Blacksmith Event", new Vector2(670, 440), Vector2.zero);
        RuntimeUiFactory.Text(panel, witch ? "THE WITCH'S BREW" : "THE BLACKSMITH", new Vector2(0, 165), new Vector2(600, 55), 32);
        status = RuntimeUiFactory.Text(panel, string.Empty, new Vector2(0, 110), new Vector2(600, 45), 22);
        if (witch)
        {
            potionButton = RuntimeUiFactory.Button(panel, "BUY DAMAGE POTION", new Vector2(0, 10), new Vector2(360, 64), BuyPotion);
        }
        else
        {
            swordButtons = new Button[2];
            swordButtons[0] = RuntimeUiFactory.Button(panel, "FORGE STEEL SWORD", new Vector2(0, 45), new Vector2(390, 60), () => CraftSword(1));
            swordButtons[1] = RuntimeUiFactory.Button(panel, "FORGE ENCHANTED SWORD", new Vector2(0, -35), new Vector2(390, 60), () => CraftSword(2));
        }
        RuntimeUiFactory.Button(panel, "CLOSE", new Vector2(0, -150), new Vector2(220, 54), () => UiManager.Instance?.HideWindow(Id));
        Refresh();
    }

    private void BuyPotion()
    {
        var profile = PlayerProgression.Instance;
        bool purchased = profile != null && profile.SpendCoins(potionPrice);
        if (purchased) profile.AddPotion();
        else if (status != null) status.text = "Not enough coins.";
        Refresh();
        if (purchased && Dino.Utility.Audio.AudioManager.Instance != null) Dino.Utility.Audio.AudioManager.Instance.PlaySound("purchase");
    }

    private void CraftSword(int sword)
    {
        var profile = PlayerProgression.Instance;
        int cost = sword == 1 ? steelSwordMaterials : enchantedSwordMaterials;
        bool crafted = profile != null && profile.CraftSword(sword, cost);
        if (crafted) profile.EquipSword(sword);
        else if (status != null) status.text = "Not enough materials or already crafted.";
        Refresh();
        if (crafted && Dino.Utility.Audio.AudioManager.Instance != null) Dino.Utility.Audio.AudioManager.Instance.PlaySound("craft");
    }

    private void Refresh()
    {
        var profile = PlayerProgression.Instance;
        if (profile == null) return;
        if (status != null) status.text = witch ? $"Price: {potionPrice} coins  |  Owned: {profile.Potions}" : $"Materials: {profile.TotalMaterials}  |  Current sword bonus: +{profile.SwordDamage:0}";
        if (potionButton != null) potionButton.interactable = profile.TotalCoins >= potionPrice;
        if (swordButtons != null)
        {
            for (int i = 0; i < swordButtons.Length; i++)
            {
                int sword = i + 1; bool owned = profile.HasSword(sword);
                swordButtons[i].interactable = owned || profile.TotalMaterials >= (sword == 1 ? steelSwordMaterials : enchantedSwordMaterials);
                var text = swordButtons[i].GetComponentInChildren<TMP_Text>();
                if (text != null) text.text = owned ? (profile.EquippedSword == sword ? "EQUIPPED SWORD" : "EQUIP SWORD") : $"FORGE SWORD ({(sword == 1 ? steelSwordMaterials : enchantedSwordMaterials)} materials)";
                swordButtons[i].onClick.RemoveAllListeners();
                swordButtons[i].onClick.AddListener(() => { if (owned) profile.EquipSword(sword); else CraftSword(sword); });
            }
        }
    }

    private void Update() { Refresh(); }
}
