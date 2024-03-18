using System;
using System.Collections.Generic;
using System.Text;
using Defines;
using Keiwando.BigInteger;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance;

    private BigInteger totalEnhanceCount;

    public BigInteger TotalEnhanceCount
    {
        get => totalEnhanceCount;
        set
        {
            onEnhanceTotal?.Invoke(value);
            totalEnhanceCount = value;
        }
    }

    private BigInteger totalWeaponComposite;
    private BigInteger totalArmorComposite;

    public BigInteger TotalWeaponComposite => totalWeaponComposite;
    public BigInteger TotalArmorComposite => totalArmorComposite;

    public event Action<BigInteger> onEnhanceTotal;
    public event Action<BigInteger> onWeaponCompositeTotal;
    public event Action<BigInteger> onArmorCompositeTotal;

    public List<WeaponInfo> weapons;
    public List<ArmorInfo> armors;

    private static Dictionary<string, Equipment> allEquipment;

    [SerializeField] private List<Sprite> weaponImages;
    [SerializeField] private List<Sprite> armorImages;

    public Dictionary<string, Sprite> images { get; private set; }

    ERarity[] rarities =
    {
        ERarity.Common,
        ERarity.Uncommon,
        ERarity.Rare,
        ERarity.Epic,
        ERarity.Legendary,
        ERarity.Mythology
    };

    [SerializeField] public Color[] rarityColors;
    [SerializeField] private Sprite[] rarityFrame;

    [SerializeField] int maxLevel = 4;
    [SerializeField] private int enhancementMaxLevel;
    public int EnhancementMaxLevel => enhancementMaxLevel;

#if UNITY_EDITOR
    [SerializeField] private int testQuantity;
#endif

    private void Awake()
    {
        instance = this;
        allEquipment = new Dictionary<string, Equipment>();
        // weapons = new List<WeaponInfo>();
        // armors = new List<ArmorInfo>();
    }

    // 장비 매니저 초기화 메서드
    public void InitEquipmentManager()
    {
        SetAllWeapons();
        SetAllArmors();
        Load();

        // TODO image setting for equipments
        // weaponImages, armorImages => images
        images = new Dictionary<string, Sprite>();
        foreach (ERarity rarity in rarities)
        {
            for (int level = 1; level <= maxLevel; level++)
            {
                string name = $"{EEquipmentType.Weapon}_{rarity}_{level}";
                images.Add(name, weaponImages[Mathf.Clamp(4 * (int)rarity + (level - 1), 0, weaponImages.Count - 1)]);
                name = $"{EEquipmentType.Armor}_{rarity}_{level}";
                images.Add(name, armorImages[Mathf.Clamp(4 * (int)rarity + (level - 1), 0, armorImages.Count - 1)]);
            }
        }

        // UIManager.instance.TryGetUI<UIEquipmentPanel>().InitializeUI();
    }

    private void Load()
    {
        totalEnhanceCount = new BigInteger(DataManager.Instance.Load<string>($"{nameof(EquipmentManager)}_{nameof(totalEnhanceCount)}", "0"));
        totalWeaponComposite = new BigInteger(DataManager.Instance.Load<string>($"{nameof(EquipmentManager)}_{nameof(totalWeaponComposite)}", "0"));
        totalArmorComposite = new BigInteger(DataManager.Instance.Load<string>($"{nameof(EquipmentManager)}_{nameof(totalArmorComposite)}", "0"));
    }

    public void Save(EEquipmentManagerSaveType type)
    {
        switch (type)
        {
            case EEquipmentManagerSaveType.TotalEnhance:
                DataManager.Instance.Save($"{nameof(EquipmentManager)}_{nameof(totalEnhanceCount)}", totalEnhanceCount.ToString());
                break;
            case EEquipmentManagerSaveType.TotalWeaponComposite:
                DataManager.Instance.Save($"{nameof(EquipmentManager)}_{nameof(totalWeaponComposite)}", totalWeaponComposite.ToString());
                break;
            case EEquipmentManagerSaveType.TotalArmorComposite:
                DataManager.Instance.Save($"{nameof(EquipmentManager)}_{nameof(totalArmorComposite)}", totalArmorComposite.ToString());
                break;
        }
    }

    public Sprite GetIcon(EEquipmentType type, int index)
    {
        switch (type)
        {
            case EEquipmentType.Weapon:
                return weaponImages[index];
            case EEquipmentType.Armor:
                return armorImages[index];
        }

        return null;
    }

    // 장비들 업데이트 하는 메서드
    void SetAllWeapons()
    {
        LoadAllWeapon();
        // if (ES3.KeyExists("Init_Game"))
        // {
        //     
        // }
        // else
        // {
        //     CreateAllWeapon();
        // }
    }

    // 방어구 업데이트하는 메서드
    private void SetAllArmors()
    {
        LoadAllArmor();
        // if (ES3.KeyExists("Init_Game"))
        // {
        //     
        // }
        // else
        // {
        //     CreateAllArmor();
        // }
    }

    // 로컬에 저장되어 있는 장비 데이터들 불러오는 메서드
    public void LoadAllWeapon()
    {
        // int weaponCount = 0;

        foreach (ERarity rarity in rarities)
        {
            var rarityIntValue = (int)rarity;
            for (int level = 1; level <= maxLevel; level++)
            {
                string name = $"{EEquipmentType.Weapon}_{rarity}_{level}";
                // TODO : create weapon information from null
                WeaponInfo weapon = weapons[4 * rarityIntValue + level - 1]; // new WeaponInfo();

                weapon.LoadEquipment();
                // if (weapon.Quantity > 0) weapon.IsOwned = true;

                // weapon.GetComponent<Button>().onClick.AddListener(() => EquipmentUI.TriggerSelectEquipment(weapon));

                AddEquipment(name, weapon);
                // weapons.Add(weapon);

                if (weapon.IsEquipped) PlayerManager.instance.EquipItem(weapon, false);

                // weaponCount++;

                // 임시
                weapon.myColor = rarityColors[rarityIntValue];
                // weapon.SetUI();

                // 보유 효과 적용 하기
                if (weapon.IsOwned)
                    PlayerManager.instance.ApplyOwnedEffect(weapon);
            }
        }
    }

    // 로컬에 저장되어 있는 방어구 데이터들 불러오는 메서드
    public void LoadAllArmor()
    {
        // int armorCount = 0;

        foreach (ERarity rarity in rarities)
        {
            var rarityIntValue = Convert.ToInt32(rarity);
            for (int level = 1; level <= maxLevel; level++)
            {
                string name = $"{EEquipmentType.Armor}_{rarity}_{level}";
                // TODO : create armor information from null
                ArmorInfo armor = armors[4 * rarityIntValue + level - 1]; //new ArmorInfo();

                armor.LoadEquipment();
                // if (armor.Quantity > 0) armor.IsOwned = true;

                // armor.GetComponent<Button>().onClick.AddListener(() => EquipmentUI.TriggerSelectEquipment(armor));

                AddEquipment(name, armor);
                // armors.Add(armor);

                if (armor.IsEquipped) PlayerManager.instance.EquipItem(armor, false);

                // armorCount++;

                // 임시
                armor.myColor = rarityColors[rarityIntValue];
                // armor.SetUI();

                // 보유 효과 적용 하기
                if (armor.IsOwned)
                    PlayerManager.instance.ApplyOwnedEffect(armor);
            }
        }
    }

    // 장비 데이터를 만드는 메서드
    public void CreateAllWeapon()
    {
        foreach (ERarity rarity in rarities)
        {
            if (rarity == ERarity.None) continue;
            var rarityIntValue = Convert.ToInt32(rarity);

            for (int level = 1; level <= maxLevel; level++)
            {
                WeaponInfo weapon = new WeaponInfo();

                string name = $"{weapon.type}_{rarity}_{level}";

                int equippedEffect = level * ((int)Mathf.Pow(10, rarityIntValue + 1));
                int ownedEffect = (int)(equippedEffect * 0.5f);

                int awakenEffect = (4 * (int)rarity + level) * 100;

                int baseEnhanceStoneRequired = (4 * (int)rarity + level) * 100;
                int baseEnhanceStoneIncrease = (4 * (int)rarity + level) * 50;

                weapon.SetWeaponInfo(name, 0, level, false, EEquipmentType.Weapon, rarity, 1, equippedEffect,
                    ownedEffect, awakenEffect, rarityColors[rarityIntValue], baseEnhanceStoneRequired,
                    baseEnhanceStoneIncrease, false);

                // AddEquipment(name, weapon);
                weapons.Add(weapon);

                // weapon.SaveEquipment(name);
            }
        }
    }

    public void CreateAllArmor()
    {
        foreach (ERarity rarity in rarities)
        {
            if (rarity == ERarity.None) continue;
            var rarityIntValue = Convert.ToInt32(rarity);
            for (int level = 1; level <= maxLevel; level++)
            {
                ArmorInfo armor = new ArmorInfo();

                string name = $"{armor.type}_{rarity}_{level}";

                int equippedEffect = level * ((int)Mathf.Pow(10, rarityIntValue + 1));
                int ownedEffect = (int)(equippedEffect * 0.5f);
                int awakenEffect = (4 * (int)rarity + level) * 100;

                int baseEnhanceStoneRequired = (4 * (int)rarity + level) * 100;
                int baseEnhanceStoneIncrease = (4 * (int)rarity + level) * 50;

                armor.SetArmorInfo(name, 0, level, false, EEquipmentType.Armor, rarity,
                    1, equippedEffect, ownedEffect, awakenEffect, rarityColors[rarityIntValue],
                    baseEnhanceStoneRequired,
                    baseEnhanceStoneIncrease, false);

                // AddEquipment(name, armor);
                armors.Add(armor);

                // armor.SaveEquipment();
            }
        }
    }

    // 매개변수로 받은 장비 합성하는 메서드
    public int Composite(Equipment equipment)
    {
        if (equipment.Quantity < 4) return -1;

        Equipment nextEquipment = TryGetNextEquipment(equipment.equipName);
        if (nextEquipment == null)
            return -1;

        int compositeCount = equipment.Quantity / 4;
        equipment.Quantity %= 4;

        if (!nextEquipment.IsOwned)
        {
            nextEquipment.IsOwned = true;
            nextEquipment.Save(ESaveType.IsOwned);
            PlayerManager.instance.ApplyOwnedEffect(nextEquipment);
        }

        nextEquipment.Quantity += compositeCount;

        return compositeCount;
    }

    public void CompositeOnce(Equipment equipment)
    {
        var status = PlayerManager.instance.status;
        var score = new BigInteger(status.BattleScore.ToString());
        if (Composite(equipment) > 0)
        {
            equipment.Save(ESaveType.Quantity);
            TryGetNextEquipment(equipment.equipName).Save(ESaveType.Quantity);
        }

        if (equipment.type == EEquipmentType.Weapon)
        {
            ++totalWeaponComposite;
            onWeaponCompositeTotal?.Invoke(totalWeaponComposite);
            Save(EEquipmentManagerSaveType.TotalWeaponComposite);
        }
        else if (equipment.type == EEquipmentType.Armor)
        {
            ++totalArmorComposite;
            onArmorCompositeTotal?.Invoke(totalArmorComposite);
            Save(EEquipmentManagerSaveType.TotalArmorComposite);
        }
        PlayerManager.instance.status.InitBattleScore();
        MessageUIManager.instance.ShowPower(status.BattleScore, status.BattleScore - score);
    }

    // 해당하는 아이템을 전체 합성한다.
    public void CompositeAll(EEquipmentType type)
    {
        var status = PlayerManager.instance.status;
        var score = new BigInteger(status.BattleScore.ToString());
        switch (type)
        {
            case EEquipmentType.Weapon:
                CompositeAllItems(weapons);
                break;
            case EEquipmentType.Armor:
                CompositeAllItems(armors);
                break;
        }

        PlayerManager.instance.status.InitBattleScore();
        MessageUIManager.instance.ShowPower(status.BattleScore, status.BattleScore - score);
    }

    // 주어진 아이템 리스트 훑으며 아이템을 합성한다.
    private void CompositeAllItems<T>(List<T> items) where T : Equipment
    {
        int count = 0;
        HashSet<T> updateItems = new HashSet<T>();
        foreach (var item in items)
        {
            var ret = Composite(item);
            if (ret < 0)
                continue;

            count += ret;
            updateItems.Add(item);
            var nextItem = TryGetNextEquipment(item.equipName);
            updateItems.Add(nextItem as T);
        }

        foreach (var item in updateItems)
        {
            // if (!item.IsOwned)
            // {
            //     item.IsOwned = true;
            //     item.Save(ESaveType.IsOwned);
            //     PlayerManager.instance.ApplyOwnedEffect(item);
            // }

            item.Save(ESaveType.Quantity);
            // item.SetQuantityUI();
        }

        var type = items[0].type;
        if (type == EEquipmentType.Weapon)
        {
            totalWeaponComposite += count;
            onWeaponCompositeTotal?.Invoke(totalWeaponComposite);
            Save(EEquipmentManagerSaveType.TotalWeaponComposite);
        }
        else if (type == EEquipmentType.Armor)
        {
            totalArmorComposite += count;
            onArmorCompositeTotal?.Invoke(totalArmorComposite);
            Save(EEquipmentManagerSaveType.TotalArmorComposite);
        }
    }

    // AllEquipment에 Equipment 더하는 메서드
    public static void AddEquipment(string equipmentName, Equipment equipment)
    {
        if (!allEquipment.ContainsKey(equipmentName))
        {
            allEquipment.Add(equipmentName, equipment);
        }
        // else
        // {
        //     Debug.LogWarning($"Weapon already exists in the dictionary: {equipmentName}");
        // }
    }

    // AllEquipment에서 매개변수로 받은 string을 key로 사용해 Equipment 찾는 매서드
    public static Equipment TryGetEquipment(string equipmentName)
    {
        if (allEquipment.TryGetValue(equipmentName, out Equipment equipment))
        {
            return equipment;
        }
        else
        {
            // Debug.LogError($"Weapon not found: {equipmentName}");
            return null;
        }
    }

    public Equipment TryGetEquipment(EEquipmentType type, int index)
    {
        switch (type)
        {
            case EEquipmentType.Weapon:
                return weapons[index];
            case EEquipmentType.Armor:
                return armors[index];
        }

        return null;
    }

    // AllEquipment에서 매개변수로 받은 key을 사용하는 Equipment 업데이트 하는 메서드
    public static void SetEquipment(string equipmentName, Equipment equipment)
    {
        Equipment targetEquipment = allEquipment[equipmentName];
        // Debug.Log("이름 : " + allEquipment[equipmentName].gameObject.name);
        targetEquipment.equippedEffect = equipment.equippedEffect;
        targetEquipment.ownedEffect = equipment.ownedEffect;
        targetEquipment.Quantity = equipment.Quantity;
        targetEquipment.enhancementLevel = equipment.enhancementLevel;

        // targetEquipment.SetQuantityUI();

        targetEquipment.SaveEquipment();
    }

    // 매개변수로 받은 key값을 사용하는 장비의 다음레벨 장비를 불러오는 메서드
    public Equipment TryGetNextEquipment(string currentKey)
    {
        int currentRarityIndex = -1;
        string currentTypeStr = "";
        int currentRarityLevel = -1;
        int maxLevel = 4; // 최대 레벨 설정

        // 현재 키에서 희귀도와 레벨 분리
        for (int i = 0; i < rarities.Length; i++)
        {
            if (currentKey.Contains("_" + rarities[i] + "_"))
            {
                currentRarityIndex = i;
                var splitKey = currentKey.Split("_" + rarities[i] + "_");
                currentTypeStr = splitKey[0];
                int.TryParse(splitKey[1], out currentRarityLevel);
                break;
            }
        }

        if (currentRarityIndex != -1 && currentRarityLevel != -1)
        {
            if (currentRarityLevel < maxLevel)
            {
                // 같은 희귀도 내에서 다음 레벨 찾기
                string nextKey = currentTypeStr + "_" + rarities[currentRarityIndex] + "_" + (currentRarityLevel + 1);
                return allEquipment.TryGetValue(nextKey, out Equipment nextEquipment) ? nextEquipment : null;
            }
            else if (currentRarityIndex < rarities.Length - 1)
            {
                // 희귀도를 증가시키고 첫 번째 레벨의 장비 찾기
                string nextKey = currentTypeStr + "_" + rarities[currentRarityIndex + 1] + "_1";
                return allEquipment.TryGetValue(nextKey, out Equipment nextEquipment) ? nextEquipment : null;
            }
        }

        // 다음 장비를 찾을 수 없는 경우
        return null;
    }

    // 매개변수로 받은 key값을 사용하는 장비의 이전레벨 장비를 불러오는 메서드
    public Equipment TryGetPreviousEquipment(string currentKey)
    {
        int currentRarityIndex = -1;
        string currentTypeStr = "";
        int currentRarityLevel = -1;

        // 현재 키에서 희귀도와 레벨 분리
        for (int i = 0; i < rarities.Length; i++)
        {
            if (currentKey.Contains("_" + rarities[i] + "_"))
            {
                currentRarityIndex = i;
                var splitKey = currentKey.Split("_" + rarities[i] + "_");
                currentTypeStr = splitKey[0];
                int.TryParse(splitKey[1], out currentRarityLevel);
                break;
            }
        }

        if (currentRarityIndex != -1 && currentRarityLevel != -1)
        {
            if (currentRarityLevel > 1)
            {
                // 같은 희귀도 내에서 이전 레벨 찾기
                string previousKey = currentTypeStr + "_" + rarities[currentRarityIndex] + "_" +
                                     (currentRarityLevel - 1);
                return allEquipment.TryGetValue(previousKey, out Equipment prevEquipment) ? prevEquipment : null;
            }
            else if (currentRarityIndex > 0)
            {
                // 희귀도를 낮추고 최대 레벨의 장비 찾기
                string previousKey = currentTypeStr + "_" + rarities[currentRarityIndex - 1] + "_4";
                return allEquipment.TryGetValue(previousKey, out Equipment prevEquipment) ? prevEquipment : null;
            }
        }

        // 이전 장비를 찾을 수 없는 경우
        return null;
    }

    public void AutoEquip(EEquipmentType selectItemType)
    {
        int index = 0;
        var typeNames = Enum.GetNames(typeof(EEquipmentType));
        for (int i = 0; i < typeNames.Length; i++)
        {
            if (typeNames[i] == selectItemType.ToString())
            {
                index = i;
                break;
            }
        }

        Equipment item;
        switch (selectItemType)
        {
            case EEquipmentType.Weapon:
                item = GetBestItem<WeaponInfo>(weapons);
                if (!ReferenceEquals(item, null))
                {
                    PlayerManager.instance.EquipItem(item, true);
                    item.Save(ESaveType.IsEquipped);
                }

                break;
            case EEquipmentType.Armor:
                item = GetBestItem<ArmorInfo>(armors);
                if (!ReferenceEquals(item, null))
                {
                    PlayerManager.instance.EquipItem(item, true);
                    item.Save(ESaveType.IsEquipped);
                }

                break;
        }
    }

    public T GetBestItem<T>(List<T> items) where T : Equipment
    {
        T best = null;
        foreach (var item in items)
        {
            if (!item.IsOwned)
                continue;
            if (ReferenceEquals(best, null))
            {
                best = item;
                continue;
            }

            if (best < item)
            {
                best = item;
            }
        }

        return best;
    }

    public Equipment TryGetBestItem(EEquipmentType type)
    {
        switch (type)
        {
            case EEquipmentType.Weapon:
                return GetBestItem(weapons);
            case EEquipmentType.Armor:
                return GetBestItem(armors);
            default:
                return null;
        }
    }

    public Sprite GetFrame(ERarity rarity)
    {
        return rarityFrame[(int)rarity];
    }

    public bool CanComposite(EEquipmentType type)
    {
        switch (type)
        {
            case EEquipmentType.Weapon:
                return CanComposite(weapons);
            case EEquipmentType.Armor:
                return CanComposite(armors);
        }

        return false;
    }

    private bool CanComposite<T>(List<T> items) where T : Equipment
    {
        foreach (var item in items)
        {
            if (item.Quantity >= 4)
                return true;
        }

        return false;
    }

    public void Enhance<T>(T item) where T : Equipment
    {
        CurrencyManager.instance.SubtractCurrency(ECurrencyType.EnhanceStone, item.GetEnhanceStone());
        item.Enhance();
        ++TotalEnhanceCount;
    }

    public sbyte CanEnhance<T>(T item) where T : Equipment
    {
        if (item.enhancementLevel >= EnhancementMaxLevel)
            return -1;

        if (CurrencyManager.instance.GetCurrency(ECurrencyType.EnhanceStone) > item.GetEnhanceStone())
            return 1;

        return 0;
    }

    public void SaveEnhanceItem(Equipment item)
    {
        item.Save(ESaveType.EnhancementLevel);
        item.Save(ESaveType.RequiredEnhanceStone);
        Save(EEquipmentManagerSaveType.TotalEnhance);
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(EquipmentManager))]
public class CustomEditorEquipmentManager : Editor
{
    private EquipmentManager manager;

    private void OnEnable()
    {
        manager = target as EquipmentManager;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("무기, 갑옷 정보 만들기"))
        {
            manager.CreateAllArmor();
            manager.CreateAllWeapon();
            EditorUtility.SetDirty(target);
        }
    }
}
#endif