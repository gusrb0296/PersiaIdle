using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using UnityEngine;

public class UIDungeonPanel : UIPanel
{
    [SerializeField] private RectTransform root;
    [SerializeField] private UIDungeonElement prefab;
    [SerializeField] private UIDungeonElementPopup popup;

    private Queue<UIDungeonElement> restUI;
    private Queue<UIDungeonElement> openedUI;

    [Header("Quest")] [SerializeField] private Transform questGuide;
    [SerializeField] private Transform goldDungeonQuestRoot;
    [SerializeField] private Transform awakenDungeonQuestRoot;
    [SerializeField] private Transform enhanceDungeonQuestRoot;

    [SerializeField] private int goldDungeonID;
    [SerializeField] private int awakenDungeonID;
    [SerializeField] private int enhanceDungeonID;

    // public ReddotRootController reddotController { get; protected set; }

    public override UIBase InitUI(UIBase parent)
    {
        base.InitUI(parent);
        restUI = new Queue<UIDungeonElement>();
        openedUI = new Queue<UIDungeonElement>();
        // reddotController = gameObject.GetComponent<ReddotRootController>();
        return this;
    }

    public override void ShowUI()
    {
        base.ShowUI();

        ShowElementUI(StageManager.instance.goldDungeon);
        ShowElementUI(StageManager.instance.awakenDungeon);
        ShowElementUI(StageManager.instance.enhanceDungeon);
    }

    private void ShowElementUI(DungeonData dungeonData)
    {
        UIDungeonElement ui = null;
        if (restUI.Count > 0)
        {
            ui = restUI.Dequeue();
        }
        else
        {
            ui = Instantiate(prefab, root);
            ui.actOnCallback += () => { restUI.Enqueue(ui); };
            ui.InitUI(this);
        }
        
        ui.ShowUI(this, dungeonData);
        openedUI.Enqueue(ui);

        Reddot dot;
        var questID = QuestManager.instance.currentQuest.GetID();
        switch (dungeonData.dungeonType)
        {
            case EDungeonType.Gold:
                goldDungeonQuestRoot = ui.GetButtonRect();
                dot = ui.reddotNode.dots[0];
                dot.type = EUpgradeType.GoldDungeon;
                ui.reddotNode.dots[0] = dot;
                if (questID >= goldDungeonID) ui.Unlock();
                else ui.Lock(goldDungeonID);
                break;
            case EDungeonType.Awaken:
                awakenDungeonQuestRoot = ui.GetButtonRect();
                dot = ui.reddotNode.dots[0];
                dot.type = EUpgradeType.AwakenDungeon;
                ui.reddotNode.dots[0] = dot;
                if (questID >= awakenDungeonID) ui.Unlock();
                else ui.Lock(awakenDungeonID);
                break;
            case EDungeonType.Enhance:
                enhanceDungeonQuestRoot = ui.GetButtonRect();
                dot = ui.reddotNode.dots[0];
                dot.type = EUpgradeType.EnhanceDungeon;
                ui.reddotNode.dots[0] = dot;
                if (questID >= enhanceDungeonID) ui.Unlock();
                else ui.Lock(enhanceDungeonID);
                break;
        }
    }

    public void ShowPopup(DungeonData dungeonData)
    {
        popup.ShowUI(this, dungeonData);
        if (questGuide.gameObject.activeInHierarchy)
        {
            questGuide.gameObject.SetActive(false);
            popup.ShowQuestRoot(QuestManager.instance.currentQuest.type);
        }
    }

    public override void CloseUI()
    {
        base.CloseUI();

        foreach (var ui in openedUI)
        {
            ui.CloseUI();
        }

        openedUI.Clear();
    }

    public override void ShowQuestRoot(EAchievementType type)
    {
        switch (type)
        {
            case EAchievementType.GoldDungeonLevel:
                questGuide.SetParent(goldDungeonQuestRoot);
                questGuide.localPosition = Vector3.zero;
                questGuide.gameObject.SetActive(true);
                QuestManager.instance.currentQuest.onComplete += x => questGuide.gameObject.SetActive(false);
                break;
            case EAchievementType.AwakenDungeonLevel:
                questGuide.SetParent(awakenDungeonQuestRoot);
                questGuide.localPosition = Vector3.zero;
                questGuide.gameObject.SetActive(true);
                QuestManager.instance.currentQuest.onComplete += x => questGuide.gameObject.SetActive(false);
                break;
            case EAchievementType.EnhanceDungeonLevel:
                questGuide.SetParent(enhanceDungeonQuestRoot);
                questGuide.localPosition = Vector3.zero;
                questGuide.gameObject.SetActive(true);
                QuestManager.instance.currentQuest.onComplete += x => questGuide.gameObject.SetActive(false);
                break;
        }
    }
}