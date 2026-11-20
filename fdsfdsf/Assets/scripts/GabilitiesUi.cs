using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GabilitiesUi : UIWindow
{
    public Button backButton;
    [SerializeField] private string gameplayWindow = "GameplayUi";
    private TMP_Text balance;
    private readonly Button[] buyButtons = new Button[3];
    private readonly string[] names = { "Sharper strikes", "Heavy blade", "Critical focus" };
    private readonly string[] descriptions = { "+1 damage per rank", "+2 damage per rank", "+10% critical chance per rank" };

    public override void Initialize()
    {
        base.Initialize();
        if (backButton != null) backButton.onClick.AddListener(() => { UiManager.Instance?.HideWindow(Id); UiManager.Instance?.ShowWindow(gameplayWindow); });
        if (CanvasRectTransform != null)
        {
            var panel = RuntimeUiFactory.Panel(CanvasRectTransform, "Skill tree", new Vector2(720, 490), Vector2.zero);
            RuntimeUiFactory.Text(panel, "ABILITIES", new Vector2(0, 190), new Vector2(600, 48), 34);
            balance = RuntimeUiFactory.Text(panel, "", new Vector2(0, 135), new Vector2(600, 42), 22);
            for (int i = 0; i < buyButtons.Length; i++)
            {
                int skill = i;
                float y = 62 - 105 * i;
                RuntimeUiFactory.Text(panel, names[i] + "\n" + descriptions[i], new Vector2(-145, y), new Vector2(300, 75), 20);
                buyButtons[i] = RuntimeUiFactory.Button(panel, "UPGRADE", new Vector2(205, y), new Vector2(220, 58), () => Buy(skill));
            }
        }
        Refresh();
    }

    private void Buy(int skill)
    {
        var p = PlayerProgression.Instance;
        if (p != null && !p.UpgradeSkill(skill)) Debug.Log("Not enough XP or maximum skill rank reached.");
        Refresh();
    }

    private void Update() { Refresh(); }

    private void Refresh()
    {
        var p = PlayerProgression.Instance;
        if (p == null || balance == null) return;
        balance.text = "Available XP: " + p.TotalXp;
        for (int i = 0; i < buyButtons.Length; i++)
        {
            if (buyButtons[i] == null) continue;
            int rank = p.GetSkillLevel(i);
            var label = buyButtons[i].GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = rank >= 3 ? "MAX RANK" : $"RANK {rank}  |  {p.GetSkillCost(i)} XP";
            buyButtons[i].interactable = rank < 3 && p.TotalXp >= p.GetSkillCost(i);
        }
    }
}
