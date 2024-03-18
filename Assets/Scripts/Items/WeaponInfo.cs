using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;

[Serializable]
public class WeaponInfo : Equipment
{
    public WeaponInfo(string equipName, int quantity, int level, bool isEquipped, EEquipmentType type, ERarity rarity,
        int enhancementLevel, int baseEquippedEffect, int baseOwnedEffect, int basicAwakenEffect,
        int baseEnhanceStoneRequired, int baseEnhanceStoneIncrease, bool isOwned = false, bool isAwaken = false) : base(
        equipName,
        quantity, level, isEquipped, type, rarity, enhancementLevel, baseEquippedEffect, baseOwnedEffect,
        basicAwakenEffect, baseEnhanceStoneRequired, baseEnhanceStoneIncrease, isOwned, isAwaken)
    {
    }

    public WeaponInfo() : base()
    {
        type = EEquipmentType.Weapon;
    }

    // 매개변수로 받은 WeaponInfo 의 정보 복사
    public void SetWeaponInfo(WeaponInfo targetInfo)
    {
        this.equipName = targetInfo.equipName;
        this.Quantity = targetInfo.Quantity;
        this.level = targetInfo.level;
        this.IsEquipped = targetInfo.IsEquipped;
        this.type = targetInfo.type;
        this.rarity = targetInfo.rarity;
        this.enhancementLevel = targetInfo.enhancementLevel;
        this.baseEquippedEffect = targetInfo.baseEquippedEffect;
        this.baseOwnedEffect = targetInfo.baseOwnedEffect;
        this.basicAwakenEffect = targetInfo.basicAwakenEffect;
        this.myColor = targetInfo.myColor;

        this.isOwned = targetInfo.isOwned;
        this.isAwaken = targetInfo.isAwaken;

        equippedEffect = this.baseEquippedEffect;
        ownedEffect = this.baseOwnedEffect;

        baseEnhanceStoneRequired = targetInfo.baseEnhanceStoneRequired;
        baseEnhanceStoneIncrease = targetInfo.baseEnhanceStoneIncrease;
        requiredEnhanceStone = new BigInteger(baseEnhanceStoneRequired);
        requiredEnhanceStone += (BigInteger)(baseEnhanceStoneIncrease) * enhancementLevel;
    }

    public void SetWeaponInfo(string name, int quantity, int level, bool OnEquipped, EEquipmentType type, ERarity eRarity,
        int enhancementLevel, int basicEquippedEffect, int basicOwnedEffect, int basicAwakenEffect, Color myColor,
        int baseEnhanceStoneRequired, int baseEnhanceStoneIncrease, bool isOwned = false, bool isAwaken = false)
    {
        this.equipName = name;
        this.Quantity = quantity;
        this.level = level;
        this.IsEquipped = OnEquipped;
        this.type = type;
        this.rarity = eRarity;
        this.enhancementLevel = enhancementLevel;
        this.baseEquippedEffect = basicEquippedEffect;
        this.baseOwnedEffect = basicOwnedEffect;
        this.basicAwakenEffect = basicAwakenEffect;
        this.myColor = myColor;

        this.isOwned = isOwned;
        this.isAwaken = isAwaken;

        equippedEffect = this.baseEquippedEffect;
        ownedEffect = this.baseOwnedEffect;

        this.baseEnhanceStoneRequired = baseEnhanceStoneRequired;
        this.baseEnhanceStoneIncrease = baseEnhanceStoneIncrease;
        requiredEnhanceStone = new BigInteger(baseEnhanceStoneRequired);
        requiredEnhanceStone += (BigInteger)(baseEnhanceStoneIncrease) * enhancementLevel;
    }
}