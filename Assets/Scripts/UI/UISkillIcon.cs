using System.Collections;
using System.Collections.Generic;
using System.Text;
using Defines;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISkillIcon : UIBase
{
    // 이미지
    [SerializeField] private Image skillImage;
    [SerializeField] private Image background;
    // 토글
    [SerializeField] public Toggle toggle;
    // 개수
    [SerializeField] private TMP_Text count;
    // 개수 슬라이더
    [SerializeField] private Slider countSlider;
    // 레벨
    [SerializeField] private TMP_Text level;
    // 레어도
    [SerializeField] private TMP_Text rarity;
    // 장착중
    [SerializeField] private GameObject equipMark;
    // 미보유 마스크
    [SerializeField] private GameObject ownMask;

    private BaseSkillData skillData;

    private RectTransform toggleRect;

    public Transform GetToggleRect()
    {
        return transform;
    }

    public void ShowUI<T>(T data, UISkillPanel uiSkillPanel) where T : BaseSkillData
    {
        // base.ShowUI();
        gameObject.SetActive(true);

        toggle.group = uiSkillPanel.toggleGroup;
        parent = uiSkillPanel;
        skillData = data;

        skillImage.sprite = SkillManager.instance.GetIcon(data.iconIndex);
        background.sprite = EquipmentManager.instance.GetFrame(data.rarity);

        level.text = $"Lv.{data.levelFrom0}";
        rarity.text = Strings.rareKor[(int)data.rarity];

        if (data is AnimSkillData anim)
        {
            equipMark.SetActive(anim.isEquipped);
        }
        else
            equipMark.SetActive(false);
        
        UpdateQuantity();
        ownMask.gameObject.SetActive(!this.skillData.isOwned);

        data.onLevelUp += UpdateLevel;
    }

    private void UpdateLevel(int currentLv)
    {
        level.text = $"Lv.{currentLv}";
    }

    public void UpdateDisplay()
    {
        if (skillData is AnimSkillData anim)
        {
            equipMark.SetActive(anim.isEquipped);
        }
        UpdateQuantity();
        ownMask.gameObject.SetActive(!skillData.isOwned);
    }

    private void UpdateQuantity()
    {
        StringBuilder sb = new StringBuilder();
        
        count.text = sb.Clear().Append(skillData.quantity).Append(" / ").Append(skillData.QuantityToLevelUp()+1).ToString();
        countSlider.maxValue = skillData.QuantityToLevelUp();
        countSlider.value = Mathf.Clamp(skillData.quantity, 0, skillData.QuantityToLevelUp()+1);
        
        // count.text = $"{skillData.quantity} / 4";
        // countSlider.value = Mathf.Clamp(skillData.quantity, 0, 4);
    }

    public BaseSkillData GetData()
    {
        return skillData;
    }

    public override void CloseUI()
    {
        base.CloseUI();
        gameObject.SetActive(false);
    }
}
