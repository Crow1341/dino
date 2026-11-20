using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackpackUi : UIWindow
{
    public Button backButton;
    [SerializeField] private string gameplayWindow = "GameplayUi";
    private TMP_Text contents;
    private Button potionButton;

    public override void Initialize()
    {
        base.Initialize();
        if (backButton != null) backButton.onClick.AddListener(() => { UiManager.Instance?.HideWindow(Id); UiManager.Instance?.ShowWindow(gameplayWindow); });
        if (CanvasRectTransform != null)
        {
            var panel = RuntimeUiFactory.Panel(CanvasRectTransform, "Inventory status", new Vector2(560, 430), Vector2.zero);
            contents = RuntimeUiFactory.Text(panel, string.Empty, new Vector2(0, 52), new Vector2(520, 300), 22);
            potionButton = RuntimeUiFactory.Button(panel, "USE POTION (+5 DAMAGE / 20 SEC)", new Vector2(0, -150), new Vector2(410, 58), UsePotion);
            Refresh();
        }
    }

    private void OnEnable() { if (PlayerProgression.Instance != null) PlayerProgression.Instance.Changed += Refresh; }
    private void OnDisable() { if (PlayerProgression.Instance != null) PlayerProgression.Instance.Changed -= Refresh; }
    private void Update() { Refresh(); }

    private void Refresh()
    {
        if (contents == null) return;
        var p = PlayerProgression.Instance;
        if (p == null) { contents.text = "Loading profile..."; return; }
        contents.text = $"COINS      {p.TotalCoins}  (run +{p.RunCoins})\nEXPERIENCE {p.TotalXp}  (run +{p.RunXp})\nMATERIALS  {p.TotalMaterials}  (run +{p.RunMaterials})\nPOTIONS    {p.Potions}\nSWORD      {(p.EquippedSword == 0 ? "Starter" : "Steel " + p.EquippedSword)}  (+{p.SwordDamage:0} damage)";
        if (potionButton != null) potionButton.interactable = p.Potions > 0;
    }

    private void UsePotion()
    {
        var manager = FindFirstObjectByType<GameplayManager>();
        if (manager != null) manager.UseDamagePotion();
        Refresh();
    }
}
