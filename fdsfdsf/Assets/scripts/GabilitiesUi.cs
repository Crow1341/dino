using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GabilitiesUi : UIWindow
{
    public Button backButton;
    [SerializeField] private string gameplayWindow = "GameplayUi";
    private TMP_Text balance;
    private readonly List<TMP_Text> descriptions = new List<TMP_Text>();
    private readonly List<Button> buyButtons = new List<Button>();
    private Button previousButton;
    private Button nextButton;
    private int page;
    private const int SkillsPerPage = 4;

    public override void Initialize()
    {
        base.Initialize();
        if (backButton != null) backButton.onClick.AddListener(() => { UiManager.Instance?.HideWindow(Id); UiManager.Instance?.ShowWindow(gameplayWindow); });
        if (CanvasRectTransform != null)
        {
            var panel = RuntimeUiFactory.Panel(CanvasRectTransform, "Skill tree", new Vector2(800, 620), Vector2.zero);
            RuntimeUiFactory.Text(panel, "HABILIDADES", new Vector2(0, 255), new Vector2(680, 48), 34);
            balance = RuntimeUiFactory.Text(panel, "", new Vector2(0, 205), new Vector2(680, 42), 22);
            for (int slot = 0; slot < SkillsPerPage; slot++)
            {
                float y = 125 - slot * 95;
                descriptions.Add(RuntimeUiFactory.Text(panel, string.Empty, new Vector2(-145, y), new Vector2(380, 78), 19));
                int selectedSlot = slot;
                buyButtons.Add(RuntimeUiFactory.Button(panel, "MEJORAR", new Vector2(230, y), new Vector2(235, 58), () => Buy(page * SkillsPerPage + selectedSlot)));
            }
            previousButton = RuntimeUiFactory.Button(panel, "ANTERIOR", new Vector2(-175, -235), new Vector2(180, 48), PreviousPage);
            nextButton = RuntimeUiFactory.Button(panel, "SIGUIENTE", new Vector2(175, -235), new Vector2(180, 48), NextPage);
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
        for (int i = 0; i < buyButtons.Count; i++)
        {
            int skill = page * SkillsPerPage + i;
            bool hasSkill = skill < PlayerProgression.SkillCount;
            descriptions[i].gameObject.SetActive(hasSkill);
            buyButtons[i].gameObject.SetActive(hasSkill);
            if (!hasSkill) continue;

            int rank = p.GetSkillLevel(skill);
            descriptions[i].text = PlayerProgression.GetSkillName(skill) + "\n" + PlayerProgression.GetSkillDescription(skill);
            var label = buyButtons[i].GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = rank >= PlayerProgression.MaxSkillRank ? "RANGO MAXIMO" : $"RANGO {rank} | {p.GetSkillCost(skill)} XP";
            buyButtons[i].interactable = rank < PlayerProgression.MaxSkillRank && p.TotalXp >= p.GetSkillCost(skill);
        }

        int lastPage = (PlayerProgression.SkillCount - 1) / SkillsPerPage;
        if (previousButton != null) previousButton.interactable = page > 0;
        if (nextButton != null) nextButton.interactable = page < lastPage;
    }

    private void PreviousPage() { page = Mathf.Max(0, page - 1); Refresh(); }
    private void NextPage() { page = Mathf.Min((PlayerProgression.SkillCount - 1) / SkillsPerPage, page + 1); Refresh(); }
}
