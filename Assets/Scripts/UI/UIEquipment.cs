using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIEquipment : UIBase
{
    private Equipment equipment;

    [SerializeField] public Toggle toggle;
    // 이미지
    [SerializeField] private Image itemIcon;
    // 색
    [SerializeField] private Image background;
    // 레벨
    [SerializeField] private TMP_Text level;
    // 개수
    [SerializeField] private TMP_Text count;
    [SerializeField] private Slider countSlider;
    // 레어도
    [SerializeField] private TMP_Text rarity;
    
    [SerializeField] private GameObject equipMark;

    private UIEquipmentPanel uiEquipmentPanel;

    public void AddListener(UnityAction<bool> action)
    {
        toggle.onValueChanged.AddListener(action);
    }

    public void SetUI(Equipment item, UIEquipmentPanel uiPanel)
    {
        uiEquipmentPanel = uiPanel;
        
        equipment = item;
        //TODO show information of item
        // 이미지
        if (EquipmentManager.instance.images.TryGetValue(item.equipName, out Sprite sprite))
            itemIcon.sprite = sprite;
        
        // 색
        // background.color = item.myColor;
        background.sprite = EquipmentManager.instance.GetFrame(item.rarity);
        // 레벨
        level.text = "Lv." + item.enhancementLevel.ToString();
        // 개수
        count.text = $"{item.Quantity}/4";
        countSlider.value = item.Quantity;
        // 레어도
        rarity.text = $"{Strings.rareKor[(int)item.rarity]} {item.level}";
        
        equipment.actOnEquipChange += UpdateEquippedMark;
        equipment.onQuantityChange += UpdateQuantityUI;
    }

    public void ShowUI(Equipment item, UIEquipmentPanel uiPanel)
    {
        // base.ShowUI();
        gameObject.SetActive(true);

        SetUI(item, uiPanel);
    }

    public override void CloseUI()
    {
        base.CloseUI();
        
        if (equipment != null)
        {
            equipment.actOnEquipChange -= UpdateEquippedMark;
            equipment.onQuantityChange -= UpdateQuantityUI;
            equipment = null;
        }
        gameObject.SetActive(false);
    }

    public Equipment GetInfo()
    {
        return equipment;
    }

    public void UpdateQuantityUI(int amount)
    {
        if (ReferenceEquals(equipment, null))
            return;
        // 개수
        count.text = amount.ToString() + "/4";
        countSlider.value = amount;
    }

    public void UpdateEnhanceLevelUI()
    {
        if (ReferenceEquals(equipment, null))
            return;
        // 레벨
        level.text = equipment.enhancementLevel.ToString();
    }

    public void UpdateEquippedMark(bool isEquip)
    {
        equipMark.SetActive(isEquip);
        if (isEquip)
            toggle.isOn = true;
    }

    public void RevealUI()
    {
        gameObject.SetActive(true);
        equipment.onOwned -= RevealUI;
    }
}