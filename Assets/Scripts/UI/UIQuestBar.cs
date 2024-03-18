using System;
using System.Collections;
using Defines;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestBar : UIBase
{
    [SerializeField] private Vector3 hidePos;
    [SerializeField] private Vector3 showPos;
    [SerializeField] private float movingDuration;

    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private Image currencyIcon;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] public Button clearBtn;
    [SerializeField] private Image reddot;
    [SerializeField] private TMP_Text questNum;

    private BaseAchievement achievement;

    [SerializeField] private Transform questGuide;

    // private bool isMoving;
    private bool isNeedShowGoal;

    protected void Awake()
    {
        InitBtn();
    }

    private void InitBtn()
    {
        clearBtn.onClick.AddListener(TryClearQuest);
    }

    private void TryClearQuest()
    {
        if (!QuestManager.instance.TryClearCurrentQuest())
        {
            MoveToCurrentQuest(QuestManager.instance.currentQuest.type);
        }
    }

    private void MoveToCurrentQuest(EAchievementType type)
    {
        switch (type)
        {
            case EAchievementType.WeaponEquip:
            {
                var ui = UIManager.instance.TryGetUI<UIEquipmentPanel>();
                ui.ShowUI();
                ui.OpenTab(EEquipmentType.Weapon);
                ui.ShowQuestRoot(type);
                break;
            }
            case EAchievementType.ArmorEquip:
            {
                var ui = UIManager.instance.TryGetUI<UIEquipmentPanel>();
                ui.ShowUI();
                ui.OpenTab(EEquipmentType.Armor);
                ui.ShowQuestRoot(type);
                break;
            }
            case EAchievementType.SkillEquip:
            case EAchievementType.SkillLevelUp:
            {
                var ui = UIManager.instance.TryGetUI<UISkillPanel>();
                ui.ShowUI();
                ui.ShowQuestRoot(type);
                break;
            }
            case EAchievementType.UseSpecialSkill:
            {
                var ui = UIManager.instance.TryGetUI<UIPlayerHealthBar>();
                ui.ShowQuestRoot(type);
                break;
            }
            case EAchievementType.GoldDungeonLevel:
            case EAchievementType.AwakenDungeonLevel:
            case EAchievementType.EnhanceDungeonLevel:
            {
                var ui = UIManager.instance.TryGetUI<UIDungeonPanel>();
                ui.ShowUI();
                ui.ShowQuestRoot(type);
                break;
            }
            case EAchievementType.StatUpgradeCount:
            case EAchievementType.AttackUpgradeCount:
            case EAchievementType.HealthUpgradeCount:
            {
                var ui = UIManager.instance.TryGetUI<UIGrowthPanel>();
                ui.ShowUI();
                ui.ChangeTab(ETrainingType.Normal);
                ui.ShowQuestRoot(type);
                break;
            }
            case EAchievementType.WeaponSummonCount:
            case EAchievementType.ArmorSummonCount:
            case EAchievementType.SkillSummonCount:
            case EAchievementType.TotalSummonCount:
            {
                var ui = UIManager.instance.TryGetUI<UISummonPanel>();
                ui.ShowUI();
                var goal = achievement.GetDescriptionGoal();

                int index = (goal % 30) == 0 ? 1 : 0;
                ui.ClickCount(1 + index);
                ui.ShowQuestRoot(type);
                break;
            }
            case EAchievementType.ClearStageLevel:
            {
                var ui = UIManager.instance.TryGetUI<UIStageBar>();
                ui.ShowQuestRoot(type);
                break;
            }
            case EAchievementType.ReachPlayerLevel:
                break;
            case EAchievementType.WeaponCompositeCount:
            case EAchievementType.EquipEnhanceCount:
            {
                var ui = UIManager.instance.TryGetUI<UIEquipmentPanel>();
                ui.ShowUI();
                ui.OpenTab(EEquipmentType.Weapon);
                ui.ShowQuestRoot(type);
                break;
            }
            case EAchievementType.ArmorCompositeCount:
            {
                var ui = UIManager.instance.TryGetUI<UIEquipmentPanel>();
                ui.ShowUI();
                ui.OpenTab(EEquipmentType.Armor);
                ui.ShowQuestRoot(type);
                break;
            }
            case EAchievementType.KillCount:
                break;
            case EAchievementType.UseSkill:
            case EAchievementType.UseAutoSkill:
            {
                var ui = UIManager.instance.TryGetUI<UISkillSlot>();
                ui.ShowQuestRoot(type);
                break;
            }
            case EAchievementType.DestinyGem:
            case EAchievementType.TempestGem:
            case EAchievementType.LightningGem:
            case EAchievementType.GuardianGem:
            case EAchievementType.RageGem:
            case EAchievementType.AbyssGem:
            {
                var ui = UIManager.instance.TryGetUI<UIGrowthPanel>();
                ui.ShowUI();
                ui.ChangeTab(ETrainingType.Awaken);
                ui.ShowQuestRoot(type);
                break;
            }
        }
    }

    // public UIQuestBar DoShowUI(float duration)
    // {
    //     StartCoroutine(WaitForDone(ShowUI));
    //     return this;
    // }
    //
    // public UIQuestBar DoCloseUI(float duration)
    // {
    //     StartCoroutine(WaitForDone(CloseUI));
    //     return this;
    // }
    //
    // IEnumerator WaitForDone(Action next)
    // {
    //     while (true)
    //     {
    //         if (!isMoving)
    //         {
    //             next.Invoke();
    //             yield break;
    //         }
    //         yield return null;
    //     }
    // }

    public override void ShowUI()
    {
        base.ShowUI();
        // isMoving = true;
        // var dotTweenerCore = Self.DOLocalMove(showPos, movingDuration);
        // dotTweenerCore.onComplete += () => { isMoving = false; };

        this.achievement = QuestManager.instance.currentQuest;

        titleText.text = achievement.GetDescription();

        isNeedShowGoal = achievement.GetDescriptionGoal() != 0;
        if (isNeedShowGoal)
        {
            countText.gameObject.SetActive(true);
            countText.text = $"{achievement.GetCount().ToString()}/{achievement.GetDescriptionGoal().ToString()}";
        }
        else
            // countText.gameObject.SetActive(false);
            countText.text = "";

        var icon = CurrencyManager.instance.GetIconOrNull(this.achievement.GetRewardType());
        if (!ReferenceEquals(icon, null))
        {
            currencyIcon.sprite = icon;
            currencyIcon.gameObject.SetActive(true);
        }
        else
            currencyIcon.gameObject.SetActive(false);

        rewardText.text = $"x{(achievement.GetRewardAmount())}";


        if (achievement.isComplete)
        {
            ShowComplete(achievement);
        }
        else
        {
            reddot.gameObject.SetActive(false);
            if (isNeedShowGoal)
                achievement.onUpdateCounter += UpdateCounter;
            achievement.onComplete += ShowComplete;
        }

        if (achievement.GetID() == 1)
            ShowQuestRoot(achievement.type);

        questNum.text = $"퀘스트 {achievement.GetID().ToString()}";
    }

    public override void CloseUI()
    {
        base.CloseUI();
        gameObject.SetActive(false);
        // isMoving = true;
        // var dotTweenerCore = Self.DOLocalMove(hidePos, movingDuration);
        // dotTweenerCore.onComplete += () => { isMoving = false; };

        if (isNeedShowGoal)
            achievement.onUpdateCounter -= UpdateCounter;
        achievement.onComplete -= ShowComplete;
    }

    private void ShowComplete(BaseAchievement notusing)
    {
        reddot.gameObject.SetActive(true);
    }

    public void UpdateCounter(int change)
    {
        countText.text = $"{achievement.GetCount().ToString()}/{achievement.GetDescriptionGoal().ToString()}";
    }

    public void HideUI()
    {
        // base.CloseUI();
        gameObject.SetActive(false);
    }

    public void DisplayUI()
    {
        gameObject.SetActive(true);
    }

    public override void ShowQuestRoot(EAchievementType type)
    {
        if (type == EAchievementType.ClickQuestBar)
        {
            questGuide.position = transform.position - Vector3.right;
            questGuide.gameObject.SetActive(true);

            QuestManager.instance.currentQuest.onComplete += (x) => questGuide.gameObject.SetActive(false);
        }
    }
}