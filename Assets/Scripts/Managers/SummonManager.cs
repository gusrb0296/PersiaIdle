using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Defines;
using Keiwando.BigInteger;
using UnityEngine;
using Random = UnityEngine.Random;

public class SummonManager : MonoBehaviour
{
    public static SummonManager instance;
    [field: SerializeField] public int diamondCostPerEquipSummon { get; private set; }
    [field: SerializeField] public int diamondCostPerSkillSummon { get; private set; }
    [field: SerializeField] public int[] SummonCountPerLevel { get; private set; }

    public float shakeProbability;
    public float shakePower;
    public int shakeVibrato;

    #region Weapon

    [field: SerializeField] public EquipSummonGacha[] weaponGachaPerLevel { get; private set; }

    public int WeaponSummonLevel
    {
        get => weaponSummonLevel;
        private set
        {
            if (weaponSummonLevel < value)
                onWeaponSummonLevelUP?.Invoke(value);
            weaponSummonLevel = value;
        }
    }

    private int weaponRewardLevel = 0;
    [SerializeField] private SummonReward[] weaponReward;

    public event Action<int> onWeaponSummonLevelUP;
    [SerializeField] private int weaponSummonLevel;

    public int WeaponSummonCount => weaponSummonCount;
    [SerializeField] private int weaponSummonCount;
    public BigInteger TotalWeaponSummonCount => totalWeaponSummonCount;
    private BigInteger totalWeaponSummonCount;
    public event Action<BigInteger> onWeaponSummonTotal;
    public event Action<int, int> onWeaponSummonCurrentAndLimit;

    #endregion

    #region Armor

    [field: SerializeField] public EquipSummonGacha[] armorGachaPerLevel { get; private set; }

    public int ArmorSummonLevel
    {
        get => armorSummonLevel;
        private set
        {
            if (armorSummonLevel < value)
                onArmorSummonLevelUP?.Invoke(value);
            armorSummonLevel = value;
        }
    }

    private int armorRewardLevel = 0;

    [SerializeField] private SummonReward[] armorReward;

    public event Action<int> onArmorSummonLevelUP;
    [SerializeField] private int armorSummonLevel;

    public int ArmorSummonCount => armorSummonCount;
    [SerializeField] private int armorSummonCount;

    public BigInteger TotalArmorSummonCount => totalArmorSummonCount;
    private BigInteger totalArmorSummonCount;
    public event Action<BigInteger> onArmorSummonTotal;
    public event Action<int, int> onArmorSummonCurrentAndLimit;

    #endregion

    #region Skill

    [field: SerializeField] public SkillSummonGacha skillGacha { get; private set; }

    public int SkillSummonLevel
    {
        get => skillSummonLevel;
        private set
        {
            if (skillSummonLevel < value)
                onSkillSummonLevelUP?.Invoke(value);
            skillSummonLevel = value;
        }
    }

    private int skillRewardLevel = 0;
    [SerializeField] private SummonReward skillReward;

    public event Action<int> onSkillSummonLevelUP;
    [SerializeField] private int skillSummonLevel;

    public int SkillSummonCount => skillSummonCount;
    [SerializeField] private int skillSummonCount;

    public BigInteger TotalSkillSummonCount => totalSkillSummonCount;
    private BigInteger totalSkillSummonCount;
    public event Action<BigInteger> onSkillSummonTotal;
    public event Action<int, int> onSkillSummonCurrentAndLimit;

    #endregion

    private void Awake()
    {
        instance = this;
    }

    public void InitSummonManager()
    {
        LoadSummonLevel();
    }

    public void StartSummonItems(EEquipmentType type, int amount, ECurrencyType currencyType, int cost)
    {
        StartCoroutine(SummonItems(type, amount, currencyType, cost));
    }

    private IEnumerator SummonItems(EEquipmentType type, int amount, ECurrencyType currencyType, int cost)
    {
        var status = PlayerManager.instance.status;
        var score = new BigInteger(status.BattleScore.ToString());
        List<SummonItem> list = new List<SummonItem>();
        LinkedList<int> upgradeIndex = new LinkedList<int>();
        HashSet<Equipment> updateItems = new HashSet<Equipment>();
        bool needSaveLevel = false;
        EquipSummonGacha gacha = null;

        switch (type)
        {
            case EEquipmentType.Weapon:
                needSaveLevel = WeaponSummonCountUpdate(amount);
                gacha = weaponGachaPerLevel[WeaponSummonLevel];
                break;
            case EEquipmentType.Armor:
                needSaveLevel = ArmorSummonCountUpdate(amount);
                gacha = armorGachaPerLevel[ArmorSummonLevel];
                break;
            default:
                gacha = weaponGachaPerLevel[0];
                break;
        }
        
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < amount; ++i)
        {
            Equipment item = EquipmentManager.TryGetEquipment(sb.Clear().Append(type.ToString()).Append("_").Append(gacha.Summon()).ToString());//$"{type}_" + weaponGachaPerLevel[WeaponSummonLevel].Summon());
            // Debug.Log(sb.ToString());
            if (!ReferenceEquals(item, null))
            {
                list.Add(new SummonItem(item));

                if (item.rarity < ERarity.Mythology && Random.Range(0f, 1f) < shakeProbability)
                    upgradeIndex.AddLast(i);
                else
                {
                    ++item.Quantity;
                    updateItems.Add(item);
                }
            }
        }

        var uiSummonPanel = UIManager.instance.TryGetUI<UISummonPanel>();
        Debug.Assert(!ReferenceEquals(uiSummonPanel, null));
        uiSummonPanel.ShowSummonList(type, list, currencyType);

        while (!uiSummonPanel.IsEndShowSummon)
            yield return null;

        while (upgradeIndex.Count > 0)
        {
            LinkedListNode<int> upIndex = upgradeIndex.First;
            int count = upgradeIndex.Count;
            while (!ReferenceEquals(upIndex, null))
            {
                if (list[upIndex.Value].item.rarity == ERarity.Mythology)
                {
                    ++list[upIndex.Value].item.Quantity;
                    updateItems.Add(list[upIndex.Value].item);
                    var del = upIndex;
                    upIndex = upIndex.Next;
                    upgradeIndex.Remove(del);
                    --count;
                    continue;
                }

                if (Random.Range(0f, 1f) < shakeProbability)
                {
                    list[upIndex.Value].IsUpgrade(true, EquipmentManager.instance.TryGetEquipment(
                            list[upIndex.Value].item.type,
                            4 * ((int)list[upIndex.Value].item.rarity + 1) + list[upIndex.Value].item.level - 1),
                        () => --count);
                    upIndex = upIndex.Next;
                    // list[target.Value].onUpgrade = EquipmentManager.instance.TryGetEquipment(target.Value.type, 4 * ((int)target.Value.rarity + 1) + target.Value.level - 1);
                }
                else
                {
                    ++list[upIndex.Value].item.Quantity;
                    updateItems.Add(list[upIndex.Value].item);

                    list[upIndex.Value].IsUpgrade(false, null, () => --count);
                    var del = upIndex;
                    upIndex = upIndex.Next;
                    upgradeIndex.Remove(del);
                }
            }

            // TODO wait until shake is finished
            while (count > 0)
                yield return null;
            yield return null;
        }

        uiSummonPanel.EndSummon();

        yield return null;

        foreach (var item in updateItems)
        {
            // item.SetQuantityUI();
            if (!item.IsOwned)
            {
                item.IsOwned = true;
                item.Save(ESaveType.IsOwned);
                PlayerManager.instance.ApplyOwnedEffect(item);
            }

            item.Save(ESaveType.Quantity);
        }

        CurrencyManager.instance.SubtractCurrency(currencyType, cost);

        SaveSummonCount(type);
        if (needSaveLevel)
            SaveSummonLevel(type);

        CurrencyManager.instance.SaveCurrencies();
        PlayerManager.instance.status.InitBattleScore();
        MessageUIManager.instance.ShowPower(status.BattleScore, status.BattleScore - score);

        yield return null;
    }

    public void StartSummonSkills(int amount, int cost)
    {
        StartCoroutine(SummonSkills(amount, cost));
    }

    private IEnumerator SummonSkills(int amount, int cost)
    {
        List<SummonSkill> list = new List<SummonSkill>();
        LinkedList<int> upgradeIndex = new LinkedList<int>();
        HashSet<BaseSkillData> updateSkill = new HashSet<BaseSkillData>();
        bool needSaveLevel = false;

        for (int i = 0; i < amount; ++i)
        {
            BaseSkillData skill = skillGacha.Summon();
            if (!ReferenceEquals(skill, null))
            {
                list.Add(new SummonSkill(skill));

                if (skill.rarity < ERarity.Mythology && Random.Range(0f, 1f) < shakeProbability)
                    upgradeIndex.AddLast(i);
                else
                {
                    ++skill.quantity;
                    updateSkill.Add(skill);
                }
            }
        }

        var uiSummonPanel = UIManager.instance.TryGetUI<UISummonPanel>();
        Debug.Assert(!ReferenceEquals(uiSummonPanel, null));
        uiSummonPanel.ShowSummonList(list);

        while (!uiSummonPanel.IsEndShowSummon)
            yield return null;

        while (upgradeIndex.Count > 0)
        {
            LinkedListNode<int> upIndex = upgradeIndex.First;
            int count = upgradeIndex.Count;
            while (!ReferenceEquals(upIndex, null))
            {
                if (Random.Range(0f, 1f) < shakeProbability)
                {
                    list[upIndex.Value].IsUpgrade(true, SummonSkillsInRarity(list[upIndex.Value].skill.rarity + 1),
                        () => --count);
                    upIndex = upIndex.Next;
                }
                else
                {
                    ++list[upIndex.Value].skill.quantity;
                    updateSkill.Add(list[upIndex.Value].skill);

                    list[upIndex.Value].IsUpgrade(false, null, () => --count);
                    var del = upIndex;
                    upIndex = upIndex.Next;
                    upgradeIndex.Remove(del);
                }
            }

            // TODO wait until shake is finished
            while (count > 0)
                yield return null;
            yield return null;
        }

        foreach (var item in updateSkill)
        {
            // item.SetQuantityUI();
            if (!item.isOwned)
            {
                item.isOwned = true;
                item.Save(ESkillDataType.IsOwned);
                if (item is PassiveSkillData passive)
                    PlayerManager.instance.AddPassiveToList(passive.status);
            }

            item.Save(ESkillDataType.Quantity);
        }

        needSaveLevel = SkillSummonCountUpdate(amount);
        CurrencyManager.instance.SubtractCurrency(ECurrencyType.Dia, cost);

        SaveSummonCount(EEquipmentType.Skill);
        if (needSaveLevel)
            SaveSummonLevel(EEquipmentType.Skill);

        CurrencyManager.instance.SaveCurrencies();

        uiSummonPanel.EndSummon();
        yield return null;
    }


    /// <summary>
    /// 해당 타입의 아이템을 개수만큼 리스트에 추가하여 돌려줍니다.
    /// </summary>
    /// <param name="type">장비 타입</param>
    /// <param name="amount">소환 개수</param>
    /// <returns>소환된 아이템 리스트</returns>
    // public bool TrySummonItems(EEquipmentType type, int amount, ECurrencyType currencyType, out List<Equipment> list)
    // {
    //     // TODO
    //     // check cost and return true/false to try summon
    //     list = new List<Equipment>();
    //     HashSet<Equipment> updateItems = new HashSet<Equipment>();
    //     bool needSaveLevel = false;
    //     int cost = 0;
    //     switch (type)
    //     {
    //         case EEquipmentType.Weapon:
    //             needSaveLevel = WeaponSummonCountUpdate(amount);
    //             switch (currencyType)
    //             {
    //                 case ECurrencyType.Dia:
    //                     cost = amount * diamondCostPerEquipSummon;
    //                     break;
    //                 case ECurrencyType.WeaponSummonTicket:
    //                     cost = amount;
    //                     break;
    //                 default:
    //                     throw new ArgumentOutOfRangeException(nameof(currencyType), currencyType, null);
    //             }
    //             while (amount > 0)
    //             {
    //                 Equipment item =
    //                     EquipmentManager.GetEquipment($"{type}_" + weaponGachaPerLevel[WeaponSummonLevel].Summon());
    //                 ++item.Quantity;
    //                 updateItems.Add(item);
    //                 list.Add(item);
    //                 --amount;
    //             }
    //             break;
    //         case EEquipmentType.Armor:
    //             needSaveLevel = ArmorSummonCountUpdate(amount);
    //             switch (currencyType)
    //             {
    //                 case ECurrencyType.Dia:
    //                     cost = amount * diamondCostPerEquipSummon;
    //                     break;
    //                 case ECurrencyType.ArmorSummonTicket:
    //                     cost = amount;
    //                     break;
    //                 default:
    //                     throw new ArgumentOutOfRangeException(nameof(currencyType), currencyType, null);
    //             }
    //             while (amount > 0)
    //             {
    //                 Equipment item =
    //                     EquipmentManager.GetEquipment($"{type}_" + armorGachaPerLevel[ArmorSummonLevel].Summon());
    //                 list.Add(item);
    //                 ++item.Quantity;
    //                 updateItems.Add(item);
    //                 --amount;
    //             }
    //             break;
    //     }
    //
    //     foreach (var item in updateItems)
    //     {
    //         // item.SetQuantityUI();
    //         item.IsOwned = true;
    //         item.SaveQuantity();
    //     }
    //     CurrencyManager.instance.SubtractCurrency(currencyType, cost)
    //
    //     SaveSummonCount(type);
    //     if (needSaveLevel)
    //         SaveSummonLevel(type);
    //     CurrencyManager.instance.SaveCurrencies();
    //     return true;
    // }
    //
    // public bool SummonSkills(int amount, out List<BaseSkillData> list)
    // {
    //     bool needSaveLevel = false;
    //     list = new List<BaseSkillData>();
    //     HashSet<BaseSkillData> updateItems = new HashSet<BaseSkillData>();
    //     int summonAmount = amount;
    //     
    //     while (amount > 0)
    //     {
    //         // TODO
    //         BaseSkillData skill = skillGacha.Summon();
    //         ++skill.quantity;
    //         list.Add(skill);
    //         updateItems.Add(skill);
    //         --amount;
    //     }
    //     needSaveLevel = SkillSummonCountUpdate(summonAmount);
    //     
    //     foreach (var item in updateItems)
    //     {
    //         item.isOwned = true;
    //         item.SaveQuantity();
    //     }
    //
    //     if (!CurrencyManager.instance.SubtractCurrency(ECurrencyType.Dia, summonAmount * diamondCostPerSkillSummon))
    //         return false;
    //
    //     SaveSummonCount(EEquipmentType.Skill);
    //     if (needSaveLevel)
    //         SaveSummonLevel(EEquipmentType.Skill);
    //     return true;
    // }
    public bool CalculateCost(EEquipmentType type, int amount, out int costDia, out int costTicket)
    {
        ECurrencyType ticket;
        if (type == EEquipmentType.Weapon)
            ticket = ECurrencyType.WeaponSummonTicket;
        else if (type == EEquipmentType.Armor)
            ticket = ECurrencyType.ArmorSummonTicket;
        else
            ticket = (ECurrencyType)99;

        var ownedTicket = CurrencyManager.instance.GetCurrency(ticket);

        int compareTicket = ownedTicket.CompareTo(amount);
        if (compareTicket >= 0) // 티켓이 많거나 같음
        {
            costTicket = amount;
            costDia = 0;
        }
        else // 티켓이 적음
        {
            costTicket = 0;
            switch (type)
            {
                case EEquipmentType.Weapon:
                case EEquipmentType.Armor:
                    costDia = amount * diamondCostPerEquipSummon;
                    break;
                case EEquipmentType.Skill:
                    costDia = amount * diamondCostPerSkillSummon;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            var current = CurrencyManager.instance.GetCurrency(ECurrencyType.Dia);
            if (costDia > current)
                return false;
        }

        return true;
    }

    public void SaveSummonLevel(EEquipmentType type)
    {
        switch (type)
        {
            case EEquipmentType.Weapon:
                DataManager.Instance.Save<int>($"{nameof(WeaponSummonLevel)}", WeaponSummonLevel);
                break;
            case EEquipmentType.Armor:
                DataManager.Instance.Save<int>($"{nameof(ArmorSummonLevel)}", ArmorSummonLevel);
                break;
            case EEquipmentType.Skill:
                DataManager.Instance.Save<int>($"{nameof(SkillSummonLevel)}", SkillSummonLevel);
                break;
        }
    }

    public void SaveSummonCount(EEquipmentType type)
    {
        switch (type)
        {
            case EEquipmentType.Weapon:
                DataManager.Instance.Save<int>($"{nameof(WeaponSummonCount)}", WeaponSummonCount);
                break;
            case EEquipmentType.Armor:
                DataManager.Instance.Save<int>($"{nameof(ArmorSummonCount)}", ArmorSummonCount);
                break;
            case EEquipmentType.Skill:
                DataManager.Instance.Save<int>($"{nameof(SkillSummonCount)}", SkillSummonCount);
                break;
        }
    }

    public void LoadSummonLevel()
    {
        weaponSummonLevel = DataManager.Instance.Load<int>($"{nameof(WeaponSummonLevel)}", 0);
        weaponSummonCount = DataManager.Instance.Load<int>($"{nameof(WeaponSummonCount)}", 0);
        weaponRewardLevel = DataManager.Instance.Load<int>($"Summon_{nameof(weaponRewardLevel)}", 0);
        armorSummonLevel = DataManager.Instance.Load<int>($"{nameof(ArmorSummonLevel)}", 0);
        armorSummonCount = DataManager.Instance.Load<int>($"{nameof(ArmorSummonCount)}", 0);
        armorRewardLevel = DataManager.Instance.Load<int>($"Summon_{nameof(armorRewardLevel)}", 0);
        skillSummonLevel = DataManager.Instance.Load<int>($"{nameof(SkillSummonLevel)}", 0);
        skillSummonCount = DataManager.Instance.Load<int>($"{nameof(SkillSummonCount)}", 0);
        skillRewardLevel = DataManager.Instance.Load<int>($"Summon_{nameof(skillRewardLevel)}", 0);

        totalWeaponSummonCount = new BigInteger(weaponSummonCount);
        for (int i = 0; i < weaponSummonLevel - 1; ++i)
        {
            totalWeaponSummonCount += SummonCountPerLevel[i];
        }

        totalArmorSummonCount = new BigInteger(armorSummonCount);
        for (int i = 0; i < armorSummonLevel - 1; ++i)
        {
            totalArmorSummonCount += SummonCountPerLevel[i];
        }

        totalSkillSummonCount = new BigInteger(skillSummonCount);
        for (int i = 0; i < skillSummonLevel - 1; ++i)
        {
            totalSkillSummonCount += SummonCountPerLevel[i];
        }
    }

    public BaseSkillData SummonSkillsInRarity(ERarity skillRarity)
    {
        var skills = SkillManager.instance.GetSkillsOnRarity(skillRarity);
        var index = Random.Range(0, skills.Count);
        // Debug.Log($"{skillRarity.ToString()} {index}/{skills.Count}");
        return skills[index];
    }


    private bool WeaponSummonCountUpdate(int amount)
    {
        bool ret = false;
        totalWeaponSummonCount += amount;
        weaponSummonCount += amount;
        while (weaponSummonCount >= SummonCountPerLevel[WeaponSummonLevel])
        {
            if (WeaponSummonLevel >= SummonCountPerLevel.Length - 1)
                break;
            weaponSummonCount -= SummonCountPerLevel[WeaponSummonLevel];
            ++WeaponSummonLevel;
            ret = true;
        }

        onWeaponSummonTotal?.Invoke(totalWeaponSummonCount);
        onWeaponSummonCurrentAndLimit?.Invoke(weaponSummonCount, SummonCountPerLevel[WeaponSummonLevel]);
        return ret;
    }

    private bool ArmorSummonCountUpdate(int amount)
    {
        bool ret = false;
        totalArmorSummonCount += amount;
        armorSummonCount += amount;
        while (armorSummonCount >= SummonCountPerLevel[ArmorSummonLevel])
        {
            if (ArmorSummonLevel >= SummonCountPerLevel.Length - 1)
                break;
            armorSummonCount -= SummonCountPerLevel[ArmorSummonLevel];
            ++ArmorSummonLevel;
            ret = true;
        }

        onArmorSummonTotal?.Invoke(totalArmorSummonCount);
        onArmorSummonCurrentAndLimit?.Invoke(armorSummonCount, SummonCountPerLevel[ArmorSummonLevel]);
        return ret;
    }

    private bool SkillSummonCountUpdate(int amount)
    {
        bool ret = false;
        totalSkillSummonCount += amount;
        skillSummonCount += amount;
        while (skillSummonCount >= 400)
        {
            skillSummonCount -= 400;
            ++SkillSummonLevel;
            ret = true;
        }

        onSkillSummonTotal?.Invoke(totalSkillSummonCount);
        onSkillSummonCurrentAndLimit?.Invoke(skillSummonCount, 400);
        return ret;
    }

    public bool TryGetSummonReward(EEquipmentType type, int summonLevel, out SummonReward rewardData)
    {
        switch (type)
        {
            case EEquipmentType.Weapon:
                if (weaponRewardLevel < summonLevel)
                {
                    rewardData = weaponReward[weaponRewardLevel];
                    ++weaponRewardLevel;
                    DataManager.Instance.Save($"Summon_{nameof(weaponRewardLevel)}", weaponRewardLevel);
                    return true;
                }
                break;
            case EEquipmentType.Armor:
                if (armorRewardLevel < summonLevel)
                {
                    rewardData = armorReward[armorRewardLevel];
                    ++armorRewardLevel;
                    DataManager.Instance.Save($"Summon_{nameof(armorRewardLevel)}", armorRewardLevel);
                    return true;
                }
                break;
            case EEquipmentType.Skill:
                if (skillRewardLevel < summonLevel)
                {
                    rewardData = skillReward;
                    ++skillRewardLevel;
                    DataManager.Instance.Save($"Summon_{nameof(skillRewardLevel)}", skillRewardLevel);
                    return true;
                }
                break;
        }

        rewardData = new SummonReward();
        return false;
    }
}

public class SummonItem
{
    public Equipment item;
    public event Action<bool, Equipment, Action> isUpgrade;

    public SummonItem(Equipment equipment)
    {
        item = equipment;
    }

    public void IsUpgrade(bool isUpgrade, Equipment to = null, Action end = null)
    {
        this.isUpgrade?.Invoke(isUpgrade, to, end);
        if (isUpgrade)
            item = to;
    }
}

public class SummonSkill
{
    public BaseSkillData skill;
    public event Action<bool, BaseSkillData, Action> isUpgrade;

    public SummonSkill(BaseSkillData skillData)
    {
        skill = skillData;
    }

    public void IsUpgrade(bool isUpgrade, BaseSkillData to = null, Action end = null)
    {
        this.isUpgrade?.Invoke(isUpgrade, to, end);
        if (isUpgrade)
            skill = to;
    }
}

[Serializable]
public class SummonReward
{
    public ECurrencyType type;
    public int amount;
}