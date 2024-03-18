 using System.Collections;
using System.Collections.Generic;
using Defines;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UISummonPercentage : UIPanel
{
    [SerializeField] private Button left;
    [SerializeField] private Button right;

    [SerializeField] private TMP_Text[] labels;
    [SerializeField] private TMP_Text[] percentages;

    private EquipSummonGacha[] equip;
    private SkillSummonGacha skill;

    private int summonLevel;
    private EEquipmentType percentageType;

    public void ShowUI(EEquipmentType type)
    {
        base.ShowUI();
        percentageType = type;
        if (type == EEquipmentType.Weapon)
        {
            summonLevel = SummonManager.instance.WeaponSummonLevel;
            equip = SummonManager.instance.weaponGachaPerLevel;
            ShowData(equip[summonLevel]);
        }
        else if (type == EEquipmentType.Armor)
        {
            summonLevel = SummonManager.instance.ArmorSummonLevel;
            equip = SummonManager.instance.armorGachaPerLevel;
            ShowData(equip[summonLevel]);
        }
        else
        {
            skill = SummonManager.instance.skillGacha;
            ShowData(skill);
        }
    }

    protected override void InitializeBtns()
    {
        base.InitializeBtns();
        
        left.onClick.AddListener(OnLeft);
        right.onClick.AddListener(OnRight);
    }

    private void OnLeft()
    {
        var next = Mathf.Max(summonLevel - 1, 0);
        if (summonLevel != next)
        {
            summonLevel = next;
            ShowData(equip[summonLevel]);
        }
    }

    private void OnRight()
    {
        var next = Mathf.Min(summonLevel + 1, equip.Length-1);
        if (summonLevel != next)
        {
            summonLevel = next;
            ShowData(equip[summonLevel]);
        }
    }

    private void ShowData(EquipSummonGacha gacha)
    {
        gacha.InitWeight();
        if (percentageType == EEquipmentType.Weapon)
            textTitles[0].text = $"무기 소환 {CustomText.SetColor($"Lv.{summonLevel}", EColorType.Green)}";
        else
            textTitles[0].text = $"갑옷 소환 {CustomText.SetColor($"Lv.{summonLevel}", EColorType.Green)}";
        for (int i = 0; i <= (int)ERarity.Mythology; ++i)
        {
            labels[i].text = Strings.rareKor[i];
            labels[i].gameObject.SetActive(true);
            percentages[i].text = $"{100 * gacha.GetPercentage((ERarity)i):F2}%";
            percentages[i].color = EquipmentManager.instance.rarityColors[i];
            labels[i].color = EquipmentManager.instance.rarityColors[i];
        }
        
        InitBtnToEquip();
    }

    private void ShowData(SkillSummonGacha gacha)
    {
        textTitles[0].text = $"스킬 소환";

        gacha.InitWeight();
        for (int i = 0; i < gacha.weightPerRarities.Length; ++i)
        {
            percentages[i].text = $"{(100 * gacha.GetPercentage((ERarity)i)):F2}%";
            percentages[i].color = EquipmentManager.instance.rarityColors[i];
            labels[i].color = EquipmentManager.instance.rarityColors[i];
        }

        for (int i = gacha.weightPerRarities.Length; i <= (int)ERarity.Mythology; ++i)
        {
            labels[i].gameObject.SetActive(false);
        }
        
        InitBtnToSkill();
    }

    private void InitBtnToEquip()
    {
        left.gameObject.SetActive(summonLevel!=0);
        right.gameObject.SetActive(summonLevel!=equip.Length-1);
    }

    private void InitBtnToSkill()
    {
        left.gameObject.SetActive(false);
        right.gameObject.SetActive(false);
    }
}
