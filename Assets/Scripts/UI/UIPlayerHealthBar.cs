using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIPlayerHealthBar : UIBase
{
    [SerializeField] private Slider hp;
    [SerializeField] private Slider mp;
    // TODO : make special skill buff mode
    [SerializeField] private Button specialSkill;
    [SerializeField] private Image specialCoolTime;
    [SerializeField] private GameObject specialFullChargeEffect;
    [SerializeField] private GameObject specialChargeEffect;

    private HealthSystem healthSystem;
    
    private BigInteger health => healthSystem.CurrentHP;
    private BigInteger maxHealth => healthSystem.CurrentMaxHP;

    private BigInteger mana => healthSystem.CurrentMP;
    private BigInteger maxMana => healthSystem.CurrentMaxMP;

    [SerializeField] private int sliderSize = 100;

    [Header("Quest")] 
    [SerializeField] private Transform questGuide;
    [SerializeField] private Transform specialQuestRoot;

    public override void ShowUI()
    {
        base.ShowUI();
        
        hp.maxValue = sliderSize;
        hp.minValue = 0;
        hp.wholeNumbers = true;

        mp.maxValue = sliderSize;
        mp.minValue = 0;
        mp.wholeNumbers = true;

        healthSystem = PlayerManager.instance.player.health;

        UpdateMana();
        UpdateHealth(healthSystem.CurrentHP, healthSystem.CurrentMaxHP);

        PlayerManager.instance.player.controller.onCurrentHPChange += UpdateHealth;
        PlayerManager.instance.player.controller.onCurrentMPChange += (x) => UpdateMana();
        
        specialSkill.onClick.AddListener(CallSpecial);
        SkillManager.instance.onSpecialTimer += UpdateSpecialSkillTimer;
    }

    private void CallSpecial()
    {
        if (PlayerManager.instance.UseSpecialSkill())
        {
            specialSkill.interactable = false;
            specialFullChargeEffect.SetActive(false);
            specialChargeEffect.SetActive(true);
        }
    }

    private void UpdateSpecialSkillTimer(float current, float full)
    {
        specialCoolTime.fillAmount = current / full;
        if (current < 0)
        {
            specialSkill.interactable = true;
            specialFullChargeEffect.SetActive(true);
            specialChargeEffect.SetActive(false);
        }
    }

    private void UpdateMana()
    {
        var result = mana * sliderSize / maxMana;
        mp.value = BigInteger.ToInt32(result);
    }

    private void UpdateHealth(BigInteger current, BigInteger max)
    {
        // Debug.Log($"health {Time.time} {health}/{maxHealth}");
        var result = current * sliderSize / max;
        hp.value = BigInteger.ToInt32(result);
    }

    public override void CloseUI()
    {
        base.CloseUI();
        
        gameObject.SetActive(false);
    }

    public override void ShowQuestRoot(EAchievementType type)
    {
        switch (type)
        {
            case EAchievementType.UseSpecialSkill:
                questGuide.SetParent(specialQuestRoot);
                questGuide.localPosition = Vector3.zero;
                questGuide.gameObject.SetActive(true);
                QuestManager.instance.currentQuest.onComplete += x => questGuide.gameObject.SetActive(false);
                break;
        }
    }
}