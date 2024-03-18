using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;
using Random = UnityEngine.Random;

public class UISummonList : UIPanel
{
    [SerializeField] private TMP_Text summonLevel;
    [SerializeField] private TMP_Text summonType;
    [SerializeField] private TMP_Text summonCurrency;
    [SerializeField] private Image summonCurrencyImage;
    [SerializeField] private Slider summonCounterSlider;
    [SerializeField] private TMP_Text summonCounter;

    [SerializeField] private Toggle autoSummon;
    private bool isAuto;
    [SerializeField] private Toggle fastSummon;
    private bool isFast;
    [SerializeField] private Button summon;
    [SerializeField] private Button skipBackground;

    [SerializeField] private UISummonListElement itemPrefab;
    [SerializeField] private RectTransform itemsRoot;
    [SerializeField] private int poolSize;
    private CustomPool<UISummonListElement> itemPool;

    private List<Equipment> summonedItemList;

    public bool isEnd;
    public bool isSkip;

    private int amount;
    private EEquipmentType type;
    private ECurrencyType currencyType;

    protected override void InitializeBtns()
    {
        base.InitializeBtns();

        summon.onClick.AddListener(() =>
        {
            if (SummonManager.instance.CalculateCost(type, amount, out int costDia, out int costTicket))
            {
                ClearUI();
                SetForStartSummon();
                switch (type)
                {
                    case EEquipmentType.Weapon:
                    case EEquipmentType.Armor:
                    {
                        int cost;
                        ECurrencyType costType;
                        if (costTicket > 0)
                        {
                            cost = costTicket;
                            costType = type == EEquipmentType.Weapon ? ECurrencyType.WeaponSummonTicket : ECurrencyType.ArmorSummonTicket;
                        }
                        else
                        {
                            cost = costDia;
                            costType = ECurrencyType.Dia;
                        }
                        SummonManager.instance.StartSummonItems(type, amount, costType, cost);
                        break;
                    }
                    case EEquipmentType.Skill:
                    {
                        SummonManager.instance.StartSummonSkills(amount, costDia);
                        break;
                    }
                }
            }
            else
            {
                MessageUIManager.instance.ShowCenterMessage(CustomText.SetColor("재화", Color.red) + "가 부족합니다.");
            }
        });
        autoSummon.onValueChanged.AddListener((onoff) => isAuto = onoff);
        fastSummon.onValueChanged.AddListener((onoff) => isFast = onoff);
        skipBackground.onClick.AddListener(() => { isSkip = true; });
    }

    public override UIBase InitUI(UIBase parent)
    {
        base.InitUI(parent);

        itemPool = EasyUIPooling.MakePool(itemPrefab, itemsRoot,
            x => x.actOnCallback += () => itemPool.Release(x),
            x => x.transform.SetAsLastSibling(),
            x => x.transform.SetAsFirstSibling(), poolSize, true);

        return this;
    }

    public void ShowUI(EEquipmentType type, List<SummonItem> items, bool isFast, ECurrencyType currencyType)
    {
        this.type = type;
        amount = items.Count;
        this.isFast = isFast;
        this.currencyType = currencyType;

        fastSummon.isOn = isFast;
        autoSummon.isOn = false;
        gameObject.SetActive(true);

        SetForStartSummon();
        SetTopBar(type);
        summonCurrency.text = CurrencyManager.instance.GetCurrencyStr(ECurrencyType.Dia);

        CurrencyManager.instance.onCurrencyChanged += SetCurrency;
        StartCoroutine(ShowSummonEffect(items, this.isFast));
    }

    private void SetCurrency(ECurrencyType moneyType, string amount)
    {
        if (moneyType == ECurrencyType.Dia)
            summonCurrency.text = CurrencyManager.instance.GetCurrencyStr(ECurrencyType.Dia);
    }

    public void ShowUI(EEquipmentType type, List<SummonSkill> skills, bool isFast, ECurrencyType currencyType)
    {
        this.type = type;
        amount = skills.Count;
        this.isFast = isFast;
        this.currencyType = currencyType;

        fastSummon.isOn = isFast;
        autoSummon.isOn = false;
        gameObject.SetActive(true);

        SetForStartSummon();
        SetTopBar(type);
        summonCurrency.text = CurrencyManager.instance.GetCurrencyStr(ECurrencyType.Dia);
        
        CurrencyManager.instance.onCurrencyChanged += SetCurrency;
        StartCoroutine(ShowSummonEffect(skills, this.isFast));
    }

    public void SetTopBar(EEquipmentType type)
    {
        switch (type)
        {
            case EEquipmentType.Weapon:
                summonLevel.gameObject.SetActive(true);
                summonLevel.text = $"Lv.{SummonManager.instance.WeaponSummonLevel}";
                summonType.text = "무기 소환";
                summonCounter.text =
                    $"{SummonManager.instance.WeaponSummonCount} / {SummonManager.instance.SummonCountPerLevel[SummonManager.instance.WeaponSummonLevel]}";
                summonCounterSlider.maxValue =
                    SummonManager.instance.SummonCountPerLevel[SummonManager.instance.WeaponSummonLevel];
                summonCounterSlider.value = SummonManager.instance.WeaponSummonCount;
                break;
            case EEquipmentType.Armor:
                summonLevel.gameObject.SetActive(true);
                summonLevel.text = $"Lv.{SummonManager.instance.ArmorSummonLevel}";
                summonType.text = "갑옷 소환";
                summonCounter.text =
                    $"{SummonManager.instance.ArmorSummonCount} / {SummonManager.instance.SummonCountPerLevel[SummonManager.instance.ArmorSummonLevel]}";
                summonCounterSlider.maxValue =
                    SummonManager.instance.SummonCountPerLevel[SummonManager.instance.ArmorSummonLevel];
                summonCounterSlider.value = SummonManager.instance.ArmorSummonCount;
                break;
            case EEquipmentType.Skill:
                summonLevel.gameObject.SetActive(false);
                summonType.text = "스킬 소환";
                summonCounter.text = $"{SummonManager.instance.SkillSummonCount} / 400";
                summonCounterSlider.maxValue = 400;
                summonCounterSlider.value = SummonManager.instance.SkillSummonCount;
                break;
            case EEquipmentType.Accessory:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    private void SetForStartSummon()
    {
        summon.interactable = false;
        foreach (var btn in exitBtns)
            btn.interactable = false;

        isEnd = false;
        isSkip = false;
    }

    public void SetForEndSummon()
    {
        SetTopBar(type);
        summon.interactable = true;
        foreach (var btn in exitBtns)
            btn.interactable = true;
    }
    
    private IEnumerator ShowSummonEffect(List<SummonSkill> skills, bool isFast)
    {
        int i = 0;
        float waitTime = 0.02f;
        if (isFast)
            isSkip = true;
        float passedTime = .0f;

        while (skills.Count > i && !isSkip)
        {
            passedTime += Time.deltaTime;
            if (passedTime < waitTime)
            {
                yield return null;
                continue;
            }

            var obj = itemPool.Get();
            obj.ShowUI(this, skills[i].skill);
            skills[i].isUpgrade += obj.Shake;

            // wait for second for high rarity item
            if (skills[i].skill.rarity >= ERarity.Epic)
                yield return new WaitForSeconds(0.7f);
            else
                yield return null;
            
            ++i;
            passedTime = .0f;
        }

        while (skills.Count > i)
        {
            var obj = itemPool.Get();
            obj.ShowUI(this, skills[i].skill);
            skills[i].isUpgrade += obj.Shake;

            ++i;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        isEnd = true;
    }

    private IEnumerator ShowSummonEffect(List<SummonItem> items, bool isFast)
    {
        int i = 0;
        float waitTime = 0.02f;
        if (isFast)
            isSkip = true;
        float passedTime = .0f;

        while (items.Count > i && !isSkip)
        {
            passedTime += Time.deltaTime;
            if (passedTime < waitTime)
            {
                yield return null;
                continue;
            }

            var obj = itemPool.Get();
            obj.ShowUI(this, items[i].item);
            items[i].isUpgrade += obj.Shake;

            // wait for second for high rarity item
            if (items[i].item.rarity >= ERarity.Epic)
                yield return new WaitForSeconds(0.7f);
            else
                yield return null;
            
            ++i;
            passedTime = .0f;
            yield return null;
        }

        while (items.Count > i)
        {
            var obj = itemPool.Get();
            obj.ShowUI(this, items[i].item);
            items[i].isUpgrade += obj.Shake;

            ++i;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        isEnd = true;
    }

    public override void CloseUI()
    {
        CurrencyManager.instance.onCurrencyChanged -= SetCurrency;
        if (isEnd)
        {
            base.CloseUI();
            ClearUI();
        }
        else
        {
            isSkip = true;
        }
    }

    public void ClearUI()
    {
        while (itemPool.UsedCount > 0)
        {
            itemPool.UsedList.First.Value.CloseUI();
        }
    }
}