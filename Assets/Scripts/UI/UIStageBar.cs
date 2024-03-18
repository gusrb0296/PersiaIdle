using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Defines;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIStageBar : UIBase
{
    [Header("Normal")]
    public GameObject[] normalUis;
    public TMP_Text stageTitle;
    public Button bossChallengeBtn;
    public Button stageChooseBtn;
    public Toggle autoBossChallengeToggle;

    [Header("진행도 UI")]
    public GameObject[] progressUis;
    public TMP_Text BossName;
    public TMP_Text BossDescription;
    public Button retireBoss;

    [Header("위 슬라이더")]
    public Image upSliderIcon;
    public TMP_Text upSliderCounter;
    public Slider upSlider;
    public int upSliderSize;

    [Header("아래 슬라이더")]
    public Image downSliderIcon;
    public TMP_Text downSliderCounter;
    public Slider downSlider;

    [Header("Quest")]
    [SerializeField] private Transform questGuide;
    [SerializeField] private Transform stageQuestRoot;

    protected void Awake()
    {
        upSlider.wholeNumbers = true;
        upSlider.minValue = 0;
        upSlider.maxValue = upSliderSize;

        downSlider.wholeNumbers = false;
        downSlider.minValue = 0;
        downSlider.maxValue = 1;
    }

    public override UIBase InitUI(UIBase parent)
    {
        base.InitUI(parent);
        InitializeBtns();
        return this;
    }

    private void InitializeBtns()
    {
        bossChallengeBtn.onClick.AddListener(OnSwitchToBossStage);
        retireBoss.onClick.AddListener(RetireBoss);
        autoBossChallengeToggle.onValueChanged.AddListener(AutoBossChallenge);
    }

    private void RetireBoss()
    {
        SetAutoBossWithoutNotify(false);
        StageManager.instance.RetireBoss();
    }

    public void SetAutoBossWithoutNotify(bool onoff)
    {
        autoBossChallengeToggle.SetIsOnWithoutNotify(onoff);
    }

    private void AutoBossChallenge(bool onoff)
    {
        StageManager.instance.AutoBossChallenge = onoff;
        if (onoff)
            OnSwitchToBossStage();
    }

    private void OnSwitchToBossStage()
    {
        if (questGuide.gameObject.activeInHierarchy)
            questGuide.gameObject.SetActive(false);
        StageManager.instance.SwitchToBossStage();
    }

    public void ShowNormalStageUI(string title)
    {
        // base.ShowUI();
        gameObject.SetActive(true);
        stageTitle.text = title;

        ChangeUI(true);
    }

    public void ShowProgressUI(EStageState type, string bossName, string bosDescription, EIconType upSliderIconType, EIconType downSliderIconType)
    {
        base.ShowUI();

        this.BossName.text = bossName;
        this.BossDescription.text = bosDescription;
        
        Sprite icon = GameManager.instance.GetIconSprite(upSliderIconType);
        this.upSliderIcon.sprite = icon;
        
        icon = GameManager.instance.GetIconSprite(downSliderIconType);
        this.downSliderIcon.sprite = icon;
        
        retireBoss.gameObject.SetActive(type == EStageState.Boss); 

        ChangeUI(false);
    }

    public void InitUpSlider(int max, int init)
    {
        upSlider.maxValue = max;
        upSlider.value = init;
        upSliderCounter.text = "0 %";
    }

    public void InitDownSlider()
    {
        downSlider.value = 1;
    }
    
    private void ChangeUI(bool isNormalUI)
    {
        foreach (var ui in normalUis)
            ui.SetActive(isNormalUI);
        foreach (var ui in progressUis)
            ui.SetActive(!isNormalUI);
    }

    public void UpdateTimer(float current, float full)
    {
        downSliderCounter.text = current.ToString("N0");
        downSlider.value = current / full;
    }

    public void UpdateHealth(BigInteger current, BigInteger full)
    {
        var health = BigInteger.ToInt32(current * upSliderSize / full);
        upSlider.value = health;
        upSliderCounter.text = $"{(health * 100 / upSliderSize).ToString()} %";
    }

    public void UpdateHealth(int current, int full)
    {
        var health = (current * upSliderSize) / full;
        upSlider.value = health;
        upSliderCounter.text = $"{current} / {full}";
    }

    public override void ShowQuestRoot(EAchievementType type)
    {
        switch (type)
        {
            case EAchievementType.ClearStageLevel:
                questGuide.SetParent(stageQuestRoot);
                questGuide.localPosition = Vector3.zero;
                questGuide.gameObject.SetActive(true);
                QuestManager.instance.currentQuest.onComplete += x => questGuide.gameObject.SetActive(false);
                break;
        }
    }
}