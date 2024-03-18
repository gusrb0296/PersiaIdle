using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using Keiwando.BigInteger;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(this);
    }

    private string nickName;

    public void SetNickName(string user)
    {
        nickName = user;
        DataManager.Instance.Save("userName", nickName);
    }

    public bool TryLoadNickName(out string userName)
    {
        if (ES3.KeyExists("userName"))
        {
            userName = DataManager.Instance.Load<string>("userName");
            return true;
        }

        userName = "";
        return false;
    }

    private void Start()
    {
        SceneManager.sceneLoaded += (scene, mode) => { if (scene.buildIndex == 1) StartGame(); };
        PushNotificationManager.instance.SetPermission();
    }

    public void StartGame()
    {
        // 순서가 매우 중요함.
        UIManager.instance.InitUIManager();
        InitRewardActions();
        SummonManager.instance.InitSummonManager();
        CurrencyManager.instance.InitCurrencyManager();
        PlayerManager.instance.InitPlayerManager(nickName);
        EquipmentManager.instance.InitEquipmentManager();
        UpgradeManager.instance.InitUpgradeManager();
        SkillManager.instance.InitSkillManager();
        StageManager.instance.InitStageManager();
        QuestManager.instance.InitQuestManager();
        MessageUIManager.instance.InitPopMessageUImanager();
        UIEffectManager.instance.InitEffectUIManager();
        // UIEffectManager.instance.InitRoot(UIManager.instance.front, UIManager.instance.panel);
        
        // PlayerManager.instance.playerCharacter.controller.onDeathEnd += StageManager.instance.ResetStage;
        PlayerInputController.instance.InitPlayerInputController();
        ReddotTree.instance.InitReddotTree();
        
        PushNotificationManager.instance.Initialize();
        OfflineTimerCtrl.instance.InitOfflineTimer();
        
        // StageManager.instance.StartGame();
        // StageManager.instance.StartSpawn(0);
        ES3.Save<bool>("Init_Game", true);
    }
    
    public Dictionary<EQuestRewardType, BaseRewardAction> dropRewards;
    
    private void InitRewardActions()
    {
        dropRewards = new Dictionary<EQuestRewardType, BaseRewardAction>();
        
        for (int i = 0; i < Enum.GetNames(typeof(EQuestRewardType)).Length; ++i)
        {
            var reward = new BaseRewardAction();
            reward.InitializeReward((EQuestRewardType)i);
            
            dropRewards.Add((EQuestRewardType)i, reward);
        }
    }

    public void GetReward(EQuestRewardType type, BigInteger amount)
    {
        dropRewards[type].GetReward(amount);
    }

    public Sprite[] iconSprite;

    public Sprite GetIconSprite(EIconType type)
    {
        if (iconSprite.Length >= (int)type)
            return iconSprite[(int)type];
        else
            throw new Exception("Icon is not enough");
    }
}
