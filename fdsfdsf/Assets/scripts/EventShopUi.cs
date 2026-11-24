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
    [SerializeField] private Sprite eventFrame;
    private TMP_Text status;
    private TMP_Text feedback;
    private Button potionButton;
    private Button[] swordButtons;

    public void Configure(bool isWitch, int potionCost, int steelCost, int enchantedCost, Sprite frame)
    {
        witch = isWitch;
        potionPrice = Mathf.Max(1, potionCost);
        steelSwordMaterials = Mathf.Max(1, steelCost);
        enchantedSwordMaterials = Mathf.Max(1, enchantedCost);
        eventFrame = frame;
    }

    public override void Initialize()
    {
        base.Initialize();
        if (CanvasRectTransform == null) return;
        var panel = RuntimeUiFactory.Panel(CanvasRectTransform, witch ? "Witch Event" : "Blacksmith Event", new Vector2(700, 470), Vector2.zero);
        CreateArtwork(panel);
        RuntimeUiFactory.Text(panel, witch ? "LA BRUJA" : "EL HERRERO", new Vector2(0, 175), new Vector2(620, 55), 34);
        status = RuntimeUiFactory.Text(panel, string.Empty, new Vector2(0, 110), new Vector2(600, 45), 22);
        feedback = RuntimeUiFactory.Text(panel, string.Empty, new Vector2(0, -105), new Vector2(610, 42), 19);
        if (witch)
        {
            potionButton = RuntimeUiFactory.Button(panel, "COMPRAR POCION DE DANO", new Vector2(0, 5), new Vector2(390, 64), BuyPotion);
        }
        else
        {
            swordButtons = new Button[2];
            swordButtons[0] = RuntimeUiFactory.Button(panel, "FORJAR ESPADA DE ACERO", new Vector2(0, 45), new Vector2(420, 60), () => CraftSword(1));
            swordButtons[1] = RuntimeUiFactory.Button(panel, "FORJAR ESPADA ENCANTADA", new Vector2(0, -35), new Vector2(420, 60), () => CraftSword(2));
        }
        RuntimeUiFactory.Button(panel, "CERRAR", new Vector2(0, -175), new Vector2(220, 54), () => UiManager.Instance?.HideWindow(Id));
        Refresh();
    }

    private void BuyPotion()
    {
        var profile = PlayerProgression.Instance;
        bool purchased = profile != null && profile.SpendCoins(potionPrice);
        if (purchased) profile.AddPotion();
        if (feedback != null) feedback.text = purchased ? "Pocion obtenida: +5 de dano durante 20 segundos." : "No tienes monedas suficientes.";
        Refresh();
        if (purchased && Dino.Utility.Audio.AudioManager.Instance != null) Dino.Utility.Audio.AudioManager.Instance.PlaySound("purchase");
    }

    private void CraftSword(int sword)
    {
        var profile = PlayerProgression.Instance;
        int cost = sword == 1 ? steelSwordMaterials : enchantedSwordMaterials;
        bool crafted = profile != null && profile.CraftSword(sword, cost);
        if (crafted) profile.EquipSword(sword);
        if (feedback != null) feedback.text = crafted ? "Espada equipada y dano actualizado." : "No tienes materiales suficientes o ya la forjaste.";
        Refresh();
        if (crafted && Dino.Utility.Audio.AudioManager.Instance != null) Dino.Utility.Audio.AudioManager.Instance.PlaySound("craft");
    }

    private void Refresh()
    {
        var profile = PlayerProgression.Instance;
        if (profile == null) return;
        if (status != null) status.text = witch
            ? $"Precio: {potionPrice} monedas | Pociones: {profile.Potions} | Efecto: +5 dano / 20 s"
            : $"Materiales: {profile.TotalMaterials} | {profile.EquippedSwordName}: +{profile.SwordDamage:0} dano";
        if (potionButton != null) potionButton.interactable = profile.TotalCoins >= potionPrice;
        if (swordButtons != null)
        {
            for (int i = 0; i < swordButtons.Length; i++)
            {
                int sword = i + 1; bool owned = profile.HasSword(sword);
                swordButtons[i].interactable = owned || profile.TotalMaterials >= (sword == 1 ? steelSwordMaterials : enchantedSwordMaterials);
                var text = swordButtons[i].GetComponentInChildren<TMP_Text>();
                if (text != null) text.text = owned
                    ? (profile.EquippedSword == sword ? "ESPADA EQUIPADA" : "EQUIPAR ESPADA")
                    : $"FORJAR ({(sword == 1 ? steelSwordMaterials : enchantedSwordMaterials)} materiales)";
                swordButtons[i].onClick.RemoveAllListeners();
                swordButtons[i].onClick.AddListener(() => { if (owned) profile.EquipSword(sword); else CraftSword(sword); });
            }
        }
    }

    private void Update() { Refresh(); }

    private void CreateArtwork(Transform panel)
    {
        if (eventFrame != null)
        {
            var artObject = new GameObject("Event frame", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            artObject.transform.SetParent(panel, false);
            var rect = (RectTransform)artObject.transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(-22f, -22f);
            rect.offsetMax = new Vector2(22f, 22f);
            var image = artObject.GetComponent<Image>();
            image.sprite = eventFrame;
            image.preserveAspect = true;
            image.color = new Color(1f, 1f, 1f, .82f);
            image.raycastTarget = false;
        }

        RuntimeUiFactory.Text(panel, witch ? "✦" : "⚒", new Vector2(-255, 170), new Vector2(80, 58), 40).raycastTarget = false;
    }
}
