using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillEquip : UIBase
{
    public Button[] slots;
    public Image[] icons;
    public Button[] exitbtns;

    private AnimSkillData target;

    [SerializeField] private RectTransform questGuide;
    [SerializeField] private RectTransform equipQuestRoot;

    private bool isPerformed;

    public override UIBase InitUI(UIBase parent)
    {
        base.InitUI(parent);
        InitializeBtns();
        equipQuestRoot = slots[0].GetComponent<RectTransform>();
        return this;
    }

    public void ShowUI(AnimSkillData selected)
    {
        base.ShowUI();
        isPerformed = false;

        target = selected;

        for (int i = 0; i < 6; ++i)
        {
            if (PlayerManager.instance.EquippedSkill[i] == null)
            {
                ShowVoidSlot(i);
            }
            else
            {
                if (PlayerManager.instance.EquippedSkill[i].skillName.Length > 0)
                    ShowSlot(i);
                else // 임시 처리. 객체가 생성되어 있는 경우가 있음.
                    ShowVoidSlot(i);
            }
        }
    }

    public override void CloseUI()
    {
        base.CloseUI();

        gameObject.SetActive(false);
    }

    private void InitializeBtns()
    {
        for (int i = 0; i < slots.Length; ++i)
        {
            var slot = i;
            slots[i].onClick.RemoveAllListeners();
            slots[i].onClick.AddListener(() => ChangeSkill(slot));
            slots[i].onClick.AddListener(() => CloseUI(1.0f));
        }

        foreach (var btn in exitbtns)
        {
            btn.onClick.AddListener(CloseUI);
        }
    }

    private void ShowSlot(int slot)
    {
        icons[slot].enabled = true;
        icons[slot].sprite = SkillManager.instance.GetIcon(PlayerManager.instance.EquippedSkill[slot].iconIndex);
    }

    private void ShowVoidSlot(int slot)
    {
        icons[slot].sprite = null;
        icons[slot].enabled = false;
    }

    private void ChangeSkill(int slot)
    {
        if (!isPerformed)
        {
            if (target.isEquipped)
            {
                ShowVoidSlot(target.equipIndex);
                SkillManager.instance.UnequipSkill(target.equipIndex);
                SkillManager.instance.EquipSkill(slot, target);
                ShowSlot(slot);
            }
            else
            {
                SkillManager.instance.EquipSkill(slot, target);
                ShowSlot(slot);
            }
            isPerformed = true;
        }
    }

    public override void ShowQuestRoot(EAchievementType type)
    {
        switch (type)
        {
            case EAchievementType.SkillEquip:
                questGuide.SetParent(equipQuestRoot);
                questGuide.localPosition = Vector3.zero;
                questGuide.gameObject.SetActive(true);
                QuestManager.instance.currentQuest.onComplete += x => questGuide.gameObject.SetActive(false);
                break;
        }
    }
}