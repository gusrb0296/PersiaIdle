using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

public class UISummonPanel : UIPanel
{
    [SerializeField] private int[] preSummonAmount;
    [SerializeField] private Toggle[] countToggles;
    private int summonAmount;
    public bool IsEndShowSummon => summonList.isEnd;

    [Header("무기 관련 UI")] 
    [SerializeField] private Button weaponSummonBtn;
    [SerializeField] private Button weaponAdSummonBtn;
    [SerializeField] private Button seeWeaponSummonPercent;
    [SerializeField] private TMP_Text weaponAdCost;
    [SerializeField] private Image weaponDiamondCostUI;
    [SerializeField] private TMP_Text weaponDiamondCost;
    [SerializeField] private Image weaponTicketCostUI;
    [SerializeField] private TMP_Text weaponTicketCost;
    // private int weaponSummonDiamondCost;
    // private int weaponSummonTicketCost;
    // private ECurrencyType weaponSummonCurrencyType;
    [SerializeField] private TMP_Text weaponSummonLevel;
    [SerializeField] private TMP_Text weaponSummonCounter;
    [SerializeField] private Slider weaponSummonSlider;
    [SerializeField] private Button weaponSummonReward;
    [SerializeField] private GameObject weaponRewardEffect;

    [Header("방어구 관련 UI")]
    [SerializeField] private Button armorSummonBtn;
    [SerializeField] private Button armorAdSummonBtn;
    [SerializeField] private Button seeArmorSummonPercent;
    [SerializeField] private TMP_Text armorAdCost;
    [SerializeField] private Image armorDiamondCostUI;
    [SerializeField] private TMP_Text armorDiamondCost;
    [SerializeField] private Image armorTicketCostUI;
    [SerializeField] private TMP_Text armorTicketCost;
    // private int armorSummonDiamondCost;
    // private int armorSummonTicketCost;
    // private ECurrencyType armorSummonCurrencyType;
    [SerializeField] private TMP_Text armorSummonLevel;
    [SerializeField] private TMP_Text armorSummonCounter;
    [SerializeField] private Slider armorSummonSlider;
    [SerializeField] private Button armorSummonReward;
    [SerializeField] private GameObject armorRewardEffect;
    
    [Header("스킬 관련 UI")]
    [SerializeField] private Button skillSummonBtn;
    [SerializeField] private Button skillAdSummonBtn;
    [SerializeField] private Button seeSkillSummonPercent;
    [SerializeField] private TMP_Text skillAdCost;
    [SerializeField] private Image skillDiamondCostUI;
    [SerializeField] private TMP_Text skillDiamondCost;
    // private int skillSummonDiamondCost;
    // [SerializeField] private TMP_Text skillSummonLevel;
    [SerializeField] private TMP_Text skillSummonCounter;
    [SerializeField] private Slider skillSummonSlider;
    [SerializeField] private Button skillSummonReward;
    [SerializeField] private GameObject skillRewardEffect;
    
    [Header("소환 리스트 UI")]
    [SerializeField] private UISummonList summonList;

    [SerializeField] private Transform questGuide;
    [SerializeField] private Transform weaponSummonQuestRoot;
    [SerializeField] private Transform armorSummonQuestRoot;
    [SerializeField] private Transform skillSummonQuestRoot;
    
    protected void Awake()
    {
        summonAmount = preSummonAmount[0];
    }

    public override void ShowUI()
    {
        base.ShowUI();
        
        weaponDiamondCostUI.sprite = CurrencyManager.instance.GetIcon(ECurrencyType.Dia);
        armorDiamondCostUI.sprite = CurrencyManager.instance.GetIcon(ECurrencyType.Dia);
        skillDiamondCostUI.sprite = CurrencyManager.instance.GetIcon(ECurrencyType.Dia);
        weaponTicketCostUI.sprite = CurrencyManager.instance.GetIcon(ECurrencyType.WeaponSummonTicket);
        armorTicketCostUI.sprite = CurrencyManager.instance.GetIcon(ECurrencyType.ArmorSummonTicket);
        
        UpdateSummonCostUI(EEquipmentType.Weapon, summonAmount);
        UpdateSummonCostUI(EEquipmentType.Armor, summonAmount);
        UpdateSummonCostUI(EEquipmentType.Skill, summonAmount);

        weaponSummonLevel.text = "Lv." + SummonManager.instance.WeaponSummonLevel;
        armorSummonLevel.text = "Lv." + SummonManager.instance.ArmorSummonLevel;

        UpdateWeaponCounter(SummonManager.instance.WeaponSummonCount, SummonManager.instance.SummonCountPerLevel[SummonManager.instance.WeaponSummonLevel]);
        UpdateArmorCounter(SummonManager.instance.ArmorSummonCount, SummonManager.instance.SummonCountPerLevel[SummonManager.instance.ArmorSummonLevel]);
        UpdateSkillCounter(SummonManager.instance.SkillSummonCount, 400);
        
        SummonManager.instance.onWeaponSummonLevelUP += UpdateWeaponLevel;
        SummonManager.instance.onArmorSummonLevelUP += UpdateArmorLevel;
        SummonManager.instance.onSkillSummonLevelUP += UpdateSkillLevel;

        SummonManager.instance.onWeaponSummonCurrentAndLimit += UpdateWeaponCounter;
        SummonManager.instance.onArmorSummonCurrentAndLimit += UpdateArmorCounter;
        SummonManager.instance.onSkillSummonCurrentAndLimit += UpdateSkillCounter;

        CurrencyManager.instance.onCurrencyChanged += CheckCurrency;
    }

    public void ClickCount(int index)
    {
        countToggles[index].isOn = true;
    }

    protected override void InitializeBtns()
    {
        base.InitializeBtns();

        for (int i = 0; i < countToggles.Length; ++i)
        {
            int index = i;
            countToggles[i].onValueChanged.AddListener((onoff) =>
            {
                if (onoff)
                {
                    summonAmount = preSummonAmount[index];
                    UpdateSummonCostUI(EEquipmentType.Weapon, summonAmount);
                    UpdateSummonCostUI(EEquipmentType.Armor, summonAmount);
                    UpdateSummonCostUI(EEquipmentType.Skill, summonAmount);
                }
            });
        }

        // 무기 관련 버튼 초기화
        weaponSummonBtn.onClick.AddListener(() => OnSummonEquipment(EEquipmentType.Weapon, summonAmount));
        // weaponAdSummonBtn.onClick.AddListener(() => OnSummonEquipment(EEquipmentType.Weapon, summonAmount));
        seeWeaponSummonPercent.onClick.AddListener(() => OnSummonPercentage(EEquipmentType.Weapon));
        weaponSummonReward.onClick.AddListener(() => TryGetSummonReward(EEquipmentType.Weapon, SummonManager.instance.WeaponSummonLevel) );
        
        // 방어구 관련 버튼 초기화
        armorSummonBtn.onClick.AddListener(() => OnSummonEquipment(EEquipmentType.Armor, summonAmount));
        // armorAdSummonBtn.onClick.AddListener(() => OnSummonEquipment(EEquipmentType.Armor, summonAmount));
        seeArmorSummonPercent.onClick.AddListener(() => OnSummonPercentage(EEquipmentType.Armor));
        armorSummonReward.onClick.AddListener(() => TryGetSummonReward(EEquipmentType.Armor, SummonManager.instance.ArmorSummonLevel));
        
        // 스킬 관련 버튼 초기화
        skillSummonBtn.onClick.AddListener(() => OnSummonSkill(summonAmount));
        // skillAdSummonBtn.onClick.AddListener(() => OnSummonSkill(summonAmount));
        seeSkillSummonPercent.onClick.AddListener(() => OnSummonPercentage(EEquipmentType.Skill));
        skillSummonReward.onClick.AddListener(() => TryGetSummonReward(EEquipmentType.Skill, SummonManager.instance.SkillSummonLevel));
    }

    private void TryGetSummonReward(EEquipmentType type, int summonLevel)
    {
        if (SummonManager.instance.TryGetSummonReward(type, summonLevel, out SummonReward reward))
        {
            // TODO Show reward panel
            UIManager.instance.TryGetUI<UIRewardPanel>().ShowUI(reward.type, reward.amount);
            // TODO 강조 표시 비활성화
            switch (type)
            {
                case EEquipmentType.Weapon:
                    weaponRewardEffect.SetActive(false);
                    break;
                case EEquipmentType.Armor:
                    armorRewardEffect.SetActive(false);
                    break;
                case EEquipmentType.Skill:
                    skillRewardEffect.SetActive(false);
                    break;
            }
        }
    }


    private void CheckCurrency(ECurrencyType type, string amount)
    {
        if (type == ECurrencyType.Dia || type == ECurrencyType.ArmorSummonTicket || type == ECurrencyType.WeaponSummonTicket)
        {
            UpdateSummonCostUI(EEquipmentType.Weapon, summonAmount);
            UpdateSummonCostUI(EEquipmentType.Armor, summonAmount);
            UpdateSummonCostUI(EEquipmentType.Skill, summonAmount);
        }
    }
    
    private void OnSummonPercentage(EEquipmentType type)
    {
        // TODO show summon percentage
        UIManager.instance.TryGetUI<UISummonPercentage>().ShowUI(type);
    }

    public void UpdateSummonCostUI(EEquipmentType type, int amount)
    {
        int costDia;
        int costTicket;
        bool onoff = SummonManager.instance.CalculateCost(type, amount, out costDia, out costTicket);

        switch (type)
        {
            case EEquipmentType.Weapon:
                if (costTicket <= 0)
                {
                    weaponTicketCostUI.gameObject.SetActive(false);
                    weaponDiamondCostUI.gameObject.SetActive(true);
                    weaponDiamondCost.text = $"x{costDia.ToString()}";
                    // weaponSummonCurrencyType = ECurrencyType.Dia;
                }
                else
                {
                    weaponTicketCostUI.gameObject.SetActive(true);
                    weaponDiamondCostUI.gameObject.SetActive(false);
                    weaponTicketCost.text = $"x{costTicket.ToString()}";
                    // weaponSummonCurrencyType = ECurrencyType.WeaponSummonTicket;
                }

                // weaponSummonDiamondCost = costDia;
                // weaponSummonTicketCost = costTicket;

                // weaponAdCost.text = $"x{amount}";
                

                //weaponSummonBtn.interactable = onoff;
                break;
            case EEquipmentType.Armor:
                if (costTicket <= 0)
                {
                    armorTicketCostUI.gameObject.SetActive(false);
                    armorDiamondCostUI.gameObject.SetActive(true);
                    armorDiamondCost.text = $"x{costDia.ToString()}";
                    // armorSummonCurrencyType = ECurrencyType.Dia;
                }
                else
                {
                    armorTicketCostUI.gameObject.SetActive(true);
                    armorDiamondCostUI.gameObject.SetActive(false);
                    armorTicketCost.text = $"x{costTicket.ToString()}";
                    // armorSummonCurrencyType = ECurrencyType.ArmorSummonTicket;
                }

                // armorSummonDiamondCost = costDia;
                // armorSummonTicketCost = costTicket;
                
                // armorAdCost.text = $"x{amount}";

                //armorSummonBtn.interactable = onoff;
                break;
            case EEquipmentType.Skill:
                skillDiamondCostUI.gameObject.SetActive(true);
                skillDiamondCost.text = $"x{costDia.ToString()}";
                // skillSummonDiamondCost = costDia;
                
                // skillAdCost.text = $"x{amount}";

                //skillSummonBtn.interactable = onoff;
                break;
        }
    }

    public void OnSummonEquipment(EEquipmentType type, int amount)
    {
        // List<Equipment> equipList;
        if (SummonManager.instance.CalculateCost(type, amount, out int costDia, out int costTicket))
        {
            if (costDia > 0)
                SummonManager.instance.StartSummonItems(type, amount, ECurrencyType.Dia, costDia);
            else if (costTicket > 0)
            {
                int currencyType = (int)ECurrencyType.WeaponSummonTicket + (int)type;
                SummonManager.instance.StartSummonItems(type, amount, (ECurrencyType)currencyType, costDia);
            }
        }
        else
        {
            MessageUIManager.instance.ShowCenterMessage(CustomText.SetColor("재화", Color.magenta)+"가 부족합니다.");
        }
        // switch (type)
        // {
        //     case EEquipmentType.Weapon:
        //         // if (SummonManager.instance.TrySummonItems(type, amount, weaponSummonCurrencyType, out equipList))
        //         // {
        //         //     // TODO
        //         //     // request summon to SummonManager and get list of them on Coroutine
        //         //     summonList.ShowUI(type, equipList, false, weaponSummonCurrencyType);
        //         // }
        //         if (SummonManager.instance.CalculateCost(type, amount, out int costDia, out int costTicket))
        //         {
        //             SummonManager.instance.StartSummonItems();
        //         }
        //         else
        //         {
        //             PopMessageUIManager.instance.ShowCenterMessage(CustomText.SetColor("재화가 부족합니다.", Color.red));
        //         }
        //         break;
        //     case EEquipmentType.Armor:
        //         if (SummonManager.instance.TrySummonItems(type, amount, armorSummonCurrencyType, out equipList))
        //         {
        //             summonList.ShowUI(type, equipList, false, armorSummonCurrencyType);
        //         }
        //         else
        //         {
        //             PopMessageUIManager.instance.ShowCenterMessage(CustomText.SetColor("재화가 부족합니다.", Color.red));
        //         }
        //         break;
        //     default:
        //         throw new ArgumentOutOfRangeException(nameof(type), type, null);
        // }
        // if (type == EEquipmentType.Weapon)
        // {
        //     CurrencyManager.instance.SubtractCurrency(ECurrencyType.WeaponSummonTicket, weaponSummonTicketCost);
        //     CurrencyManager.instance.SubtractCurrency(ECurrencyType.Dia, weaponSummonDiamondCost);
        // }
        // else if (type == EEquipmentType.Armor)
        // {
        //     CurrencyManager.instance.SubtractCurrency(ECurrencyType.ArmorSummonTicket, armorSummonTicketCost);
        //     CurrencyManager.instance.SubtractCurrency(ECurrencyType.Dia, armorSummonDiamondCost);
        // }
        
    }

    public void OnSummonSkill(int amount)
    {
        if (SummonManager.instance.CalculateCost(EEquipmentType.Skill, amount, out int costDia, out int costTicket))
        {
            SummonManager.instance.StartSummonSkills(amount, costDia);
        }
        else
        {
            MessageUIManager.instance.ShowCenterMessage(CustomText.SetColor("재화", Color.magenta)+"가 부족합니다.");
        }
        // if (SummonManager.instance.SummonSkills(amount, out List<BaseSkillData> skillList))
            // summonList.ShowUI(EEquipmentType.Skill, skillList, false);
        
        // CurrencyManager.instance.SubtractCurrency(ECurrencyType.Dia, skillSummonDiamondCost);
    }

    public override void CloseUI()
    {
        base.CloseUI();

        SummonManager.instance.onWeaponSummonLevelUP -= UpdateWeaponLevel;
        SummonManager.instance.onArmorSummonLevelUP -= UpdateArmorLevel;
        SummonManager.instance.onSkillSummonLevelUP -= UpdateSkillLevel;

        SummonManager.instance.onWeaponSummonCurrentAndLimit -= UpdateWeaponCounter;
        SummonManager.instance.onArmorSummonCurrentAndLimit -= UpdateArmorCounter;
        SummonManager.instance.onSkillSummonCurrentAndLimit -= UpdateSkillCounter;

        CurrencyManager.instance.onCurrencyChanged -= CheckCurrency;
    }
    
    private void UpdateWeaponLevel(int level)
    {
        weaponSummonLevel.text = "Lv." + level;
        // TODO 보상 버튼 강조
        weaponRewardEffect.SetActive(true);
    }

    private void UpdateArmorLevel(int level)
    {
        armorSummonLevel.text = "Lv." + level;
        // TODO 보상 버튼 강조
        armorRewardEffect.SetActive(true);
    }

    private void UpdateSkillLevel(int level)
    {
        // TODO 보상 버튼 강조
        skillRewardEffect.SetActive(true);
    }

    private void UpdateWeaponCounter(int current, int max)
    {
        weaponSummonCounter.text = $"{current} / {max}";
        weaponSummonSlider.maxValue = max;
        weaponSummonSlider.value = current; 
    }

    private void UpdateArmorCounter(int current, int max)
    {
        armorSummonCounter.text = $"{current} / {max}";
        armorSummonSlider.maxValue = max;
        armorSummonSlider.value = current;
    }

    private void UpdateSkillCounter(int current, int max)
    {
        skillSummonCounter.text = $"{current} / {max}";
        skillSummonSlider.maxValue = max;
        skillSummonSlider.value = current;
    }

    public override void ShowQuestRoot(EAchievementType type)
    {
        switch (type)
        {
            case EAchievementType.WeaponSummonCount:
                questGuide.SetParent(weaponSummonQuestRoot);
                questGuide.localPosition = Vector3.zero;
                // questGuide.position = weaponSummonQuestRoot.position;
                break;
            case EAchievementType.ArmorSummonCount:
                questGuide.SetParent(armorSummonQuestRoot);
                questGuide.localPosition = Vector3.zero;
                // questGuide.position = armorSummonQuestRoot.position;
                break;
            case EAchievementType.SkillSummonCount:
                questGuide.SetParent(skillSummonQuestRoot);
                questGuide.localPosition = Vector3.zero;
                // questGuide.position = skillSummonQuestRoot.position;
                break;
        }
        questGuide.gameObject.SetActive(true);
        QuestManager.instance.currentQuest.onComplete += x=>questGuide.gameObject.SetActive(false);
    }

    public void ShowSummonList(EEquipmentType type, List<SummonItem> equipList, ECurrencyType currencyType)
    {
        summonList.ShowUI(type, equipList, false, currencyType);
    }

    public void ShowSummonList(List<SummonSkill> skillList)
    {
        summonList.ShowUI(EEquipmentType.Skill, skillList, false, ECurrencyType.Dia);
    }

    public void EndSummon()
    {
        summonList.SetForEndSummon();
    }
}