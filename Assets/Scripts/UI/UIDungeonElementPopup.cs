using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

public class UIDungeonElementPopup : UIBase
{
    public Button[] exitBtns;
    public ENormalRewardType rewardType;
    public TMP_Text ownedReward;
    public Image ownedRewardIcon;
    public TMP_Text dungeonType;
    public TMP_Text subTitle;
    public TMP_Text goalInstruction;
    public TMP_Text description;
    public Image rewardIcon;
    public TMP_Text rewardAmount;
    public TMP_Text enemyAttack;
    public TMP_Text enemyHealth;
    public TMP_Text invitationCount;
    public Button enterBtn;

    private DungeonData data;

    [Header("Quest")]
    [SerializeField] private RectTransform questGuide;
    [SerializeField] private RectTransform enterQuestRoot;

    protected void Awake()
    {
        InitializeBtns();
    }

    public override void CloseUI()
    {
        base.CloseUI();
        
        gameObject.SetActive(false);
    }

    public void ShowUI(UIDungeonPanel _parent, DungeonData _data)
    {
        base.ShowUI();
        this.parent = _parent;
        this.data = _data;

        this.rewardType = data.rewardType;
        ownedRewardIcon.sprite = CurrencyManager.instance.GetIcon((ECurrencyType)data.rewardType);
        ownedReward.text = CurrencyManager.instance.GetCurrency((ECurrencyType)data.rewardType).ChangeToShort();
        dungeonType.text = Strings.dungeonToKOR[(int)data.dungeonType];
        subTitle.text = data.dungeonSubName + CustomText.SetColor($" Lv.{data.dungeonLevel}", data.invitationType);
        goalInstruction.text = data.goalInstruction;
        description.text = data.description;
        rewardIcon.sprite = CurrencyManager.instance.GetIcon((ECurrencyType)data.rewardType);
        rewardAmount.text = data.GetTotalReward().ChangeToShort();
        enemyAttack.text = data.GetEnemyAttack().ChangeToShort();
        enemyHealth.text = data.GetEnemyHealth().ChangeToShort();
        invitationCount.text = $"{CurrencyManager.instance.GetCurrencyStr(data.invitationType)}/1";
    }

    public void InitializeBtns()
    {
        foreach (var btn in exitBtns)
            btn.onClick.AddListener(CloseUI);
        
        enterBtn.onClick.AddListener(TryEnterDungeon);
    }

    private void TryEnterDungeon()
    {
        if (CurrencyManager.instance.GetCurrency(data.invitationType) > 0)
        {
            CloseUI();
            parent.CloseUI();
            StageManager.instance.InitToDungeon(data);
            UIManager.instance.TryGetUI<UIBottomMenuCtrl>().ActivateDungeonBtn(true);
        }
        else
        {
            MessageUIManager.instance.ShowCenterMessage(CustomText.SetColor("입장권", Color.red)+"이 부족합니다.");
        }
    }

    public override void ShowQuestRoot(EAchievementType type)
    {
        switch (type)
        {
            case EAchievementType.GoldDungeonLevel:
            case EAchievementType.AwakenDungeonLevel:
                if (data.dungeonType == EDungeonType.Gold)
                {
                    questGuide.SetParent(enterQuestRoot);
                }
                else if (data.dungeonType == EDungeonType.Awaken)
                {
                    questGuide.SetParent(enterQuestRoot);
                }
                else if (data.dungeonType == EDungeonType.Enhance)
                {
                    // TODO 관련 퀘스트가 있다면 작업이 필요함.
                    return;
                }
                questGuide.localPosition = Vector3.zero;
                questGuide.gameObject.SetActive(true);
                QuestManager.instance.currentQuest.onComplete += x => questGuide.gameObject.SetActive(false);
                break;
        }
    }
}
