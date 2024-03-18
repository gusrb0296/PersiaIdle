using System.Text;
using Defines;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UIEnhancePopup : UIPanel
{
    [SerializeField] private HoldCheckerButton enhanceBtn;
    private Equipment equipment;

    // rare
    [SerializeField] private Image backEffect;

    // image
    [SerializeField] private Image itemImage;

    // 강화 레벨
    [SerializeField] private TMP_Text titleText;
    
    [SerializeField] private TMP_Text rarityText;

    // cost
    [SerializeField] private Image costImage;
    [SerializeField] private TMP_Text costText;

    // currency
    [SerializeField] private Image currencyImage;
    [SerializeField] private TMP_Text currencyText;

    // effect
    [SerializeField] private TMP_Text ownedEffectText;
    [SerializeField] private TMP_Text equipedEffectText;

    [SerializeField] private Transform questGuide;

    public void ShowUI(Equipment item)
    {
        base.ShowUI();

        costImage.sprite = CurrencyManager.instance.GetIcon(ECurrencyType.EnhanceStone);
        currencyImage.sprite = CurrencyManager.instance.GetIcon(ECurrencyType.EnhanceStone);

        equipment = item;
        // backEffect.color = item.myColor;
        backEffect.sprite = EquipmentManager.instance.GetFrame(item.rarity);

        if (EquipmentManager.instance.images.TryGetValue(item.equipName, out Sprite sprite))
            itemImage.sprite = sprite;

        rarityText.text = $"{Strings.rareKor[(int)item.rarity]} {item.level}";

        titleText.text = $"장비 강화 ( {item.enhancementLevel} / {EquipmentManager.instance.EnhancementMaxLevel} )";

        UpdateCostAndCurrency();
    }

    protected override void InitializeBtns()
    {
        base.InitializeBtns();

        enhanceBtn.onClick.AddListener(TryEnhanceItem);
        enhanceBtn.onExit.AddListener(SaveEnhanceItem);
    }

    private void SaveEnhanceItem()
    {
        EquipmentManager.instance.SaveEnhanceItem(equipment);
        CurrencyManager.instance.SaveCurrencies();
    }

    private void TryEnhanceItem()
    {
        var ret = EquipmentManager.instance.CanEnhance(equipment);
        if (ret == 1)
        {
            EquipmentManager.instance.Enhance(equipment);
            UpdateCostAndCurrency();
        }
        else if (ret == 0)
        {
            MessageUIManager.instance.ShowCenterMessage("강화석이 부족합니다.");
        }
        else if (ret == -1)
        {
            MessageUIManager.instance.ShowCenterMessage("최대 레벨입니다.");
        }
        Debug.Assert(ret is >= -1 and <= 1, "Not Defined");
        // failed enhance item
    }

    private void UpdateCostAndCurrency()
    {
        var cost = equipment.GetEnhanceStone();
        costText.text = cost.ChangeToShort();

        var currency = CurrencyManager.instance.GetCurrency(ECurrencyType.EnhanceStone).ChangeToShort();
        currencyText.text = currency;

        StringBuilder sb = new StringBuilder();
        
        switch (equipment.type)
        {
            case EEquipmentType.Weapon:
                sb.Clear().Append("보유 효과 : ").Append("피해량 +").Append(equipment.ownedEffect).Append(CustomText.SetColor(" (\u25b2", EColorType.Green))
                    .Append(CustomText.SetColor((equipment.ownedEffect + equipment.baseOwnedEffect).ChangeToShort(),
                        EColorType.Green)).Append(") 증가");
                ownedEffectText.text = sb.ToString();
                
                sb.Clear().Append("장착 효과 : ").Append("피해량 +").Append(equipment.equippedEffect).Append(CustomText.SetColor(" (\u25b2", EColorType.Green))
                    .Append(CustomText.SetColor((equipment.equippedEffect + equipment.baseEquippedEffect).ChangeToShort(),
                        EColorType.Green)).Append(") 증가");
                equipedEffectText.text = sb.ToString();
                break;
            case EEquipmentType.Armor:
                sb.Clear().Append("보유 효과 : ").Append("체력 +").Append(equipment.ownedEffect).Append(CustomText.SetColor("% (\u25b2", EColorType.Green))
                    .Append(CustomText.SetColor((equipment.ownedEffect + equipment.baseOwnedEffect).ChangeToShort(),
                        EColorType.Green)).Append(") 증가");
                ownedEffectText.text = sb.ToString();
                
                sb.Clear().Append("장착 효과 : ").Append("체력 +").Append(equipment.equippedEffect).Append(CustomText.SetColor(" (\u25b2", EColorType.Green))
                    .Append(CustomText.SetColor((equipment.equippedEffect + equipment.baseEquippedEffect).ChangeToShort(),
                        EColorType.Green)).Append(") 증가");
                equipedEffectText.text = sb.ToString();
                break;
        }
        
        // ownedEffectText.text = $"보유 효과 : {(equipment.type == EEquipmentType.Weapon ? "피해량" : "체력")} {equipment.ownedEffect}%" + CustomText.SetColor($"=>({equipment.ownedEffect + equipment.baseOwnedEffect}%)", CustomText.CustomColor(30, 255, 30)) + " 증가";
        
        // equipedEffectText.text = $"장착 효과 : {(equipment.type == EEquipmentType.Weapon ? "피해량" : "체력")} {equipment.equippedEffect}%" + CustomText.SetColor($"=>({equipment.equippedEffect + equipment.baseEquippedEffect}%)", CustomText.CustomColor(30, 255, 30)) + " 증가";
        
        titleText.text = $"장비 강화 ( {equipment.enhancementLevel} / {EquipmentManager.instance.EnhancementMaxLevel} )";
    }

    public override void ShowQuestRoot(EAchievementType type)
    {
        switch (type)
        {
            case EAchievementType.EquipEnhanceCount:
                questGuide.position = enhanceBtn.transform.position;
                questGuide.gameObject.SetActive(true);
                QuestManager.instance.currentQuest.onComplete += (x) => questGuide.gameObject.SetActive(false);
                break;
        }
    }
}