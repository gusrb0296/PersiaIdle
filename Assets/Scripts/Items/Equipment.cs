using System;
using Defines;
using UnityEngine;
using Keiwando.BigInteger;
using UnityEngine.Serialization;

[Serializable]
public class Equipment
{
    // public static string[] rareKor = { "일반", "희귀", "레어", "에픽", "전설", "신화" };//, "고대" };

    public string equipName;
    public event Action<int> onQuantityChange;
    public int Quantity
    {
        get => quantity;
        set
        {
            onQuantityChange?.Invoke(value);
            quantity = value;
        }
    }

    public int quantity;
    public int level;

    public bool IsEquipped
    {
        get => isEquipped;
        set
        {
            isEquipped = value;
            actOnEquipChange?.Invoke(isEquipped);
        }
    }

    protected bool isEquipped;

    public EEquipmentType type;
    public ERarity rarity;
    public int enhancementLevel;
    public int baseEquippedEffect;
    public BigInteger equippedEffect;
    public int baseOwnedEffect;
    public BigInteger ownedEffect;
    public static int enhancementMaxLevel = 100;
    public int basicAwakenEffect;
    public bool isAwaken;

    public bool IsOwned
    {
        get => isOwned;
        set
        {
            if (value)
            {
                isOwned = value;
                onOwned?.Invoke();
            }
        }
    }

    public event Action onOwned;

    protected bool isOwned;
    public Color myColor;

    public Action<bool> actOnEquipChange;

    // 장비 강화 관련
    public int baseEnhanceStoneRequired;
    public int baseEnhanceStoneIncrease;
    [FormerlySerializedAs("requipredEnhanceStone")] public BigInteger requiredEnhanceStone;

    public void SetInfo(Equipment equip)
    {
        equipName = equip.equipName;
        Quantity = equip.Quantity;
        level = equip.level;
        IsEquipped = equip.IsEquipped;
        type = equip.type;
        rarity = equip.rarity;
        enhancementLevel = equip.enhancementLevel;
        baseEquippedEffect = equip.baseEquippedEffect;
        baseOwnedEffect = equip.baseOwnedEffect;

        isOwned = equip.isOwned;

        equippedEffect = equip.equippedEffect;
        ownedEffect = equip.ownedEffect;

        basicAwakenEffect = equip.basicAwakenEffect;
        isAwaken = equip.isAwaken;

        baseEnhanceStoneRequired = equip.baseEnhanceStoneRequired;
        baseEnhanceStoneIncrease = equip.baseEnhanceStoneIncrease;
        requiredEnhanceStone = new BigInteger(baseEnhanceStoneRequired);
        requiredEnhanceStone += (BigInteger)(baseEnhanceStoneIncrease) * enhancementLevel;

        myColor = equip.myColor;
    }

    public Equipment(string equipName, int quantity, int level, bool isEquipped, EEquipmentType type, ERarity rarity,
        int enhancementLevel, int baseEquippedEffect, int baseOwnedEffect, int basicAwakenEffect,
        int baseEnhanceStoneRequired, int baseEnhanceStoneIncrease, bool isOwned = false, bool isAwaken = false)
    {
        this.equipName = equipName;
        this.Quantity = quantity;
        this.level = level;
        this.IsEquipped = isEquipped;
        this.type = type;
        this.rarity = rarity;
        this.enhancementLevel = enhancementLevel;
        this.baseEquippedEffect = baseEquippedEffect;
        this.baseOwnedEffect = baseOwnedEffect;

        this.isOwned = isOwned;
        this.isAwaken = isAwaken;

        equippedEffect = this.baseEquippedEffect;
        ownedEffect = this.baseOwnedEffect;

        this.baseEnhanceStoneRequired = baseEnhanceStoneRequired;
        this.baseEnhanceStoneIncrease = baseEnhanceStoneIncrease;
        requiredEnhanceStone = new BigInteger(baseEnhanceStoneRequired);
        requiredEnhanceStone += (BigInteger)(baseEnhanceStoneIncrease) * enhancementLevel;
    }

    public Equipment()
    {
        equippedEffect = new BigInteger();
        ownedEffect = new BigInteger();
    }

    // 강화 메서드
    public virtual void Enhance()
    {
        // EquipmentManager.instance.TotalEnhanceCount += 1;

        // Currency manager에서 강화석 빼기
        // if (enhancementLevel >= EquipmentManager.instance.EnhancementMaxLevel)
        //     return false;
        //
        // if (!CurrencyManager.instance.SubtractCurrency(ECurrencyType.EnhanceStone, requipredEnhanceStone))
        //     return false;
        
        equippedEffect += baseEquippedEffect;
        ownedEffect += baseOwnedEffect;
        
        enhancementLevel++;
        
        requiredEnhanceStone += (baseEnhanceStoneIncrease);

        // Save(ESaveType.EnhancementLevel);
        // Save(ESaveType.RequiredEnhanceStone);

        // return true;
    }

    // 강화할 때 필요한 강화석 return 시키는 메서드
    public BigInteger GetEnhanceStone()
    {
        return requiredEnhanceStone;
    }

    // 개수 체크하는 메서드
    public bool CheckQuantity()
    {
        if (Quantity >= 4)
        {
            return true;
        }

        return false;
    }

    // 장비 데이터를 ES3 파일에 저장
    public void SaveEquipment()
    {
        // DataManager.Instance.Save<string>("name_" + equipName, equipName);
        DataManager.Instance.Save<int>("quantity_" + equipName, Quantity);
        // DataManager.Instance.Save<int>("level_" + equipName, level);
        DataManager.Instance.Save<bool>("isEquiped_" + equipName, IsEquipped);
        // DataManager.Instance.Save<EEquipmentType>("type_" + equipName, type);
        // DataManager.Instance.Save<ERarity>("rarity_" + equipName, rarity);
        DataManager.Instance.Save<int>("enhancementLevel_" + equipName, enhancementLevel);
        // DataManager.Instance.Save<int>("basicEquippedEffect_" + equipName, basicEquippedEffect);
        // DataManager.Instance.Save<int>("basicOwnedEffect_" + equipName, basicOwnedEffect);
        // DataManager.Instance.Save<int>("basicAwakenEffect_" + equipName, basicAwakenEffect);
        DataManager.Instance.Save<bool>("isOwned_" + equipName, isOwned);

        DataManager.Instance.Save<string>("equippedEffect_" + equipName, equippedEffect.ToString());
        DataManager.Instance.Save<string>("ownedEffect_" + equipName, ownedEffect.ToString());

        // DataManager.Instance.Save<int>("baseEnhanceStoneRequired_" + equipName, baseEnhanceStoneRequired);
        // DataManager.Instance.Save<int>("baseEnhanceStoneIncrease_" + equipName, baseEnhanceStoneIncrease);
        DataManager.Instance.Save<string>($"{requiredEnhanceStone}_{equipName}", requiredEnhanceStone.ToString());
    }

    public void Save(ESaveType type)
    {
        switch (type)
        {
            case ESaveType.Quantity:
                DataManager.Instance.Save<int>("quantity_" + equipName, Quantity);
                break;
            case ESaveType.IsEquipped:
                DataManager.Instance.Save<bool>("isEquiped_" + equipName, IsEquipped);
                break;
            case ESaveType.EnhancementLevel:
                DataManager.Instance.Save<int>("enhancementLevel_" + equipName, enhancementLevel);
                DataManager.Instance.Save<string>("equippedEffect_" + equipName, equippedEffect.ToString());
                DataManager.Instance.Save<string>("ownedEffect_" + equipName, ownedEffect.ToString());
                break;
            case ESaveType.IsOwned:
                DataManager.Instance.Save<bool>("isOwned_" + equipName, isOwned);
                break;
            case ESaveType.EquippedEffect:
                DataManager.Instance.Save<string>("equippedEffect_" + equipName, equippedEffect.ToString());
                break;
            case ESaveType.OwnedEffect:
                DataManager.Instance.Save<string>("ownedEffect_" + equipName, ownedEffect.ToString());
                break;
            case ESaveType.RequiredEnhanceStone:
                DataManager.Instance.Save<string>($"{requiredEnhanceStone}_{equipName}", requiredEnhanceStone.ToString());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    // 장비 데이터를 ES3 파일에서 불러오기
    public void LoadEquipment()
    {
        // if (!ES3.KeyExists("name_" + equipName)) return;

        // Debug.Log("장비 정보 불러오기 " + equipName);

        // equipName = DataManager.Instance.Load<string>("name_" + equipName);
        Quantity = DataManager.Instance.Load<int>("quantity_" + equipName, 0);
        // level = DataManager.Instance.Load<int>("level_" + equipName);
        IsEquipped = DataManager.Instance.Load<bool>("isEquiped_" + equipName, false);
        // type = DataManager.Instance.Load<EEquipmentType>("type_" + equipName);
        // rarity = DataManager.Instance.Load<ERarity>("rarity_" + equipName);
        enhancementLevel = DataManager.Instance.Load<int>("enhancementLevel_" + equipName, 0);
        // basicEquippedEffect = DataManager.Instance.Load<int>("basicEquippedEffect_" + equipName);
        // basicOwnedEffect = DataManager.Instance.Load<int>("basicOwnedEffect_" + equipName);
        // basicAwakenEffect = DataManager.Instance.Load<int>("basicAwakenEffect_" + equipName);
        isOwned = DataManager.Instance.Load<bool>("isOwned_" + equipName, false);
        if (Quantity > 0) isOwned = true;

        equippedEffect = new BigInteger(DataManager.Instance.Load<string>($"{nameof(equippedEffect)}_{equipName}", baseEquippedEffect.ToString()));
        ownedEffect = new BigInteger(DataManager.Instance.Load<string>($"{nameof(ownedEffect)}_{equipName}", baseOwnedEffect.ToString()));

        // baseEnhanceStoneRequired = DataManager.Instance.Load<int>("baseEnhanceStoneRequired_" + equipName);
        // baseEnhanceStoneIncrease = DataManager.Instance.Load<int>("baseEnhanceStoneIncrease_" + equipName);
        
        requiredEnhanceStone = new BigInteger(DataManager.Instance.Load<string>($"{nameof(requiredEnhanceStone)}_{equipName}", baseEnhanceStoneRequired.ToString()));
        // requipredEnhanceStone = new BigInteger(baseEnhanceStoneRequired);
        // requipredEnhanceStone += (BigInteger)(baseEnhanceStoneIncrease) * enhancementLevel;
    }

    public void LoadEquipment(string equipmentID)
    {
        // if (!ES3.KeyExists("name_" + equipmentID)) return;

        // Debug.Log("장비 정보 불러오기 " + equipmentID);

        // equipName = DataManager.Instance.Load<string>("name_" + equipmentID);
        Quantity = DataManager.Instance.Load<int>("quantity_" + equipmentID, 0);
        // level = DataManager.Instance.Load<int>("level_" + equipmentID);
        IsEquipped = DataManager.Instance.Load<bool>("isEquiped_" + equipmentID, false);
        // type = DataManager.Instance.Load<EEquipmentType>("type_" + equipmentID);
        // rarity = DataManager.Instance.Load<ERarity>("rarity_" + equipmentID);
        enhancementLevel = DataManager.Instance.Load<int>("enhancementLevel_" + equipmentID, 0);
        // basicEquippedEffect = DataManager.Instance.Load<int>("basicEquippedEffect_" + equipmentID);
        // basicOwnedEffect = DataManager.Instance.Load<int>("basicOwnedEffect_" + equipmentID);
        // basicAwakenEffect = DataManager.Instance.Load<int>("basicAwakenEffect_" + equipmentID);
        isOwned = DataManager.Instance.Load<bool>("isOwned_" + equipmentID, false);

        equippedEffect = new BigInteger(DataManager.Instance.Load<string>("equippedEffect_" + equipmentID, baseEquippedEffect.ToString()));
        ownedEffect = new BigInteger(DataManager.Instance.Load<string>("ownedEffect_" + equipmentID, baseOwnedEffect.ToString()));

        // baseEnhanceStoneRequired = DataManager.Instance.Load<int>("baseEnhanceStoneRequired_" + equipmentID);
        // baseEnhanceStoneIncrease = DataManager.Instance.Load<int>("baseEnhanceStoneIncrease_" + equipmentID);
        
        requiredEnhanceStone = new BigInteger(DataManager.Instance.Load<string>($"{nameof(requiredEnhanceStone)}_{equipmentID}", baseEnhanceStoneRequired.ToString()));
        // requipredEnhanceStone += (BigInteger)(baseEnhanceStoneIncrease) * enhancementLevel;
    }

    public static bool operator <(Equipment a, Equipment b)
    {
        if (a.equippedEffect < b.equippedEffect)
            return true;
        else
            return false;
    }

    public static bool operator >(Equipment a, Equipment b)
    {
        if (a.equippedEffect > b.equippedEffect)
            return true;
        else
            return false;
    }
}