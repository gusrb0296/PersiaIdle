using System;
using System.Collections.Generic;
using System.Linq;
using Defines;
using Keiwando.BigInteger;
using UnityEngine;

public class ReddotTree : MonoBehaviour
{
    public static ReddotTree instance;

    public bool[] reddotState { get; protected set; }

    public LinkedList<ReddotNode> openedDots;
    private void Awake()
    {
        instance = this;
        openedDots = new LinkedList<ReddotNode>();
        reddotState = new bool[Enum.GetValues(typeof(EUpgradeType)).Length];
    }

    public void InitReddotTree()
    {
        if (ReferenceEquals(openedDots, null))
        {
            openedDots = new LinkedList<ReddotNode>();
            reddotState = new bool[Enum.GetValues(typeof(EUpgradeType)).Length];
        }
        Subscribe();
        CheckAll();
    }

    private void CheckAll()
    {
        CheckOnSkillSummon(0);
        CheckOnOffEquip(null, PlayerManager.instance.EquippedArmor);
        CheckOnOffEquip(null, PlayerManager.instance.EquippedWeapon);
        
        CheckCurrency(ECurrencyType.Gold, CurrencyManager.instance.GetCurrencyStr(ECurrencyType.Gold));
        CheckCurrency(ECurrencyType.Dia, CurrencyManager.instance.GetCurrencyStr(ECurrencyType.Dia));
        CheckCurrency(ECurrencyType.AwakenStone, CurrencyManager.instance.GetCurrencyStr(ECurrencyType.AwakenStone));
        CheckCurrency(ECurrencyType.GoldInvitation, CurrencyManager.instance.GetCurrencyStr(ECurrencyType.GoldInvitation));
        CheckCurrency(ECurrencyType.AwakenInvitation, CurrencyManager.instance.GetCurrencyStr(ECurrencyType.AwakenInvitation));
    }

    private void Subscribe()
    {
        CurrencyManager.instance.onCurrencyChanged += CheckCurrency;
        SummonManager.instance.onArmorSummonTotal += CheckOffArmorSummon;
        SummonManager.instance.onWeaponSummonTotal += CheckOffWeaponSummon;
        EquipmentManager.instance.onArmorCompositeTotal += CheckArmorComposite;
        EquipmentManager.instance.onWeaponCompositeTotal += CheckWeaponComposite;
        PlayerManager.instance.onEquipItem += CheckOnOffEquip;

        SummonManager.instance.onSkillSummonTotal += CheckOnSkillSummon;
        PlayerManager.instance.onEquipSkill += OffSkillEquip;
        PlayerManager.instance.onUnequipSkill += OnSkillUnequip;

        UpgradeManager.instance.onTrainingTypeAndCurrentLevel += OffTrainUpgrade;
        UpgradeManager.instance.onAwakenUpgrade += OffAwakenUpgrade;
    }

    private void Unsubscribe()
    {
        CurrencyManager.instance.onCurrencyChanged -= CheckCurrency;
        SummonManager.instance.onArmorSummonTotal -= CheckOffArmorSummon;
        SummonManager.instance.onWeaponSummonTotal -= CheckOffWeaponSummon;
        EquipmentManager.instance.onArmorCompositeTotal -= CheckArmorComposite;
        EquipmentManager.instance.onWeaponCompositeTotal -= CheckWeaponComposite;
        PlayerManager.instance.onEquipItem -= CheckOnOffEquip;

        SummonManager.instance.onSkillSummonTotal -= CheckOnSkillSummon;
        PlayerManager.instance.onEquipSkill -= OffSkillEquip;
        PlayerManager.instance.onUnequipSkill -= OnSkillUnequip;

        UpgradeManager.instance.onTrainingTypeAndCurrentLevel -= OffTrainUpgrade;
        UpgradeManager.instance.onAwakenUpgrade -= OffAwakenUpgrade;
    }


    private void OffAwakenUpgrade(EStatusType type, int level)
    {
        TurnOnOffReddot(EUpgradeType.Awaken, false);
    }

    private void OffTrainUpgrade(EStatusType type, int level)
    {
        TurnOnOffReddot(EUpgradeType.Training, false);
    }

    private void OffSkillEquip(int slot, AnimSkillData equipSkill)
    {
        TurnOnOffReddot(EUpgradeType.SkillEquip, false);
    }

    private void OnSkillUnequip(int slot)
    {
        TurnOnOffReddot(EUpgradeType.SkillEquip, true);
    }

    private void CheckOnSkillSummon(BigInteger total)
    {
        int count = 0;
        foreach (var skill in PlayerManager.instance.EquippedSkill)
        {
            if (!ReferenceEquals(skill, null)) ++count;
        }

        if (count >= 6) return;

        if (count < SkillManager.instance.GetSkillOwnCount(ESkillType.Active))
            TurnOnOffReddot(EUpgradeType.SkillEquip, true);
    }

    private void CheckOnOffEquip(Equipment from, Equipment to)
    {
        if (ReferenceEquals(from, to)) return;
        
        switch (to.type)
        {
            case EEquipmentType.Weapon:
            {
                var best = EquipmentManager.instance.TryGetBestItem(EEquipmentType.Weapon);
                if (ReferenceEquals(best, null)) return;
                TurnOnOffReddot(EUpgradeType.WeaponEquip, to.equipName != best.equipName);
                break;
            }
            case EEquipmentType.Armor:
            {
                var best = EquipmentManager.instance.TryGetBestItem(EEquipmentType.Armor);
                if (ReferenceEquals(best, null)) return;
                TurnOnOffReddot(EUpgradeType.ArmorEquip, to.equipName != best.equipName);
                break;
            }
        }
    }

    private void CheckArmorComposite(BigInteger compositeAmount)
    {
        if (!ReferenceEquals(PlayerManager.instance.EquippedArmor,
                EquipmentManager.instance.TryGetBestItem(EEquipmentType.Armor)))
            TurnOnOffReddot(EUpgradeType.ArmorEquip, true);
    }

    private void CheckWeaponComposite(BigInteger compositeAmount)
    {
        if (!ReferenceEquals(PlayerManager.instance.EquippedWeapon,
                EquipmentManager.instance.TryGetBestItem(EEquipmentType.Weapon)))
            TurnOnOffReddot(EUpgradeType.WeaponEquip, true);
    }

    private void CheckOffWeaponSummon<T>(T value)
    {
        var best = EquipmentManager.instance.TryGetBestItem(EEquipmentType.Weapon);
        
        if (ReferenceEquals(best, null) || !ReferenceEquals(PlayerManager.instance.EquippedWeapon, best))
            TurnOnOffReddot(EUpgradeType.WeaponEquip, true);

        TurnOnOffReddot(EUpgradeType.SummonWeapon, false);
    }

    private void CheckCurrency(ECurrencyType type, string currencyAmount)
    {
        var currency = new BigInteger(currencyAmount);
        if (type == ECurrencyType.Gold)
        {
            if (UpgradeManager.instance.statUpgradeInfo.Any(status => status.cost < currency))
            {
                TurnOnOffReddot(EUpgradeType.Training, true);
            }
        }
        else if (type == ECurrencyType.Dia)
        {
            if (currency > SummonManager.instance.diamondCostPerEquipSummon)
            {
                TurnOnOffReddot(EUpgradeType.SummonArmor, true);
                TurnOnOffReddot(EUpgradeType.SummonWeapon, true);
            }
            else
            {
                TurnOnOffReddot(EUpgradeType.SummonArmor, false);
                TurnOnOffReddot(EUpgradeType.SummonWeapon, false);
            }

            if (currency > SummonManager.instance.diamondCostPerSkillSummon)
            {
                TurnOnOffReddot(EUpgradeType.SummonSkill, true);
            }
            else
            {
                TurnOnOffReddot(EUpgradeType.SummonSkill, false);
            }
        }
        else if (type == ECurrencyType.AwakenStone)
        {
            if (UpgradeManager.instance.awakenUpgradeInfo.Any(status => status.cost < currency))
            {
                TurnOnOffReddot(EUpgradeType.Awaken, true);
            }
        }
        else if (type == ECurrencyType.GoldInvitation)
        {
            TurnOnOffReddot(EUpgradeType.GoldDungeon, true);
        }
        else if (type == ECurrencyType.AwakenInvitation)
        {
            TurnOnOffReddot(EUpgradeType.AwakenDungeon, true);
        }
    }

    private void CheckOffArmorSummon(BigInteger total)
    {
        var best = EquipmentManager.instance.TryGetBestItem(EEquipmentType.Armor);
        
        if (ReferenceEquals(best, null) || !ReferenceEquals(PlayerManager.instance.EquippedArmor, best))
            TurnOnOffReddot(EUpgradeType.ArmorEquip, true);

        TurnOnOffReddot(EUpgradeType.SummonArmor, false);
    }

    public void TurnOnOffReddot(EUpgradeType type, bool onoff)
    {
        if (reddotState[(int)type] == onoff)
            return;
        
        reddotState[(int)type] = onoff;
        foreach (var reddot in openedDots)
        {
            reddot.TurnOnOffReddot(type, onoff);
        }
    }
}