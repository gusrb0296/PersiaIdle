using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UISkillSlot : UIBase
{
    public Toggle autoSkill;
    public Button[] skillSlots;
    public Image[] skillSlotIcons;
    public Image[] skillSlotTimers;

    [SerializeField] private Transform questGuide;

    public override void CloseUI()
    {
        base.CloseUI();
    }

    //protected override void Awake()
    //{
    //    base.Awake();
    //}

    public override UIBase InitUI(UIBase parent)
    {
        //return base.InitUI(parent);
        InitializeBtns();
        return this;
    }

    public void ShowUI(int slot, AnimSkillData skillData)
    {
        //base.ShowUI();
        skillSlotIcons[slot].sprite = SkillManager.instance.GetIcon(skillData.iconIndex);
        skillSlotIcons[slot].gameObject.SetActive(true);
        skillSlotTimers[slot].gameObject.SetActive(true);
    }

    public override void ShowUI()
    {
        base.ShowUI();
        autoSkill.SetIsOnWithoutNotify(SkillManager.instance.IsAutoSkill);
    }

    public void ShowVoidUI(int slot)
    {
        skillSlotIcons[slot].gameObject.SetActive(false);
        skillSlotTimers[slot].gameObject.SetActive(false);
        skillSlotTimers[slot].fillAmount = 0;
    }

    public void UpdateTimer(int slot, float time, float fullTime)
    {
        skillSlotTimers[slot].fillAmount = time / fullTime;
        if (time < 0)
            skillSlots[slot].interactable = true;
    }

    public void InitializeBtns()
    {
        for (int i = 0; i < skillSlots.Length; ++i)
        {
            int index = i;
            skillSlots[index].onClick.AddListener(() => UseSkill(index));
        }

        autoSkill.onValueChanged.AddListener((onoff) => { SkillManager.instance.IsAutoSkill = onoff; });
    }

    public void UseSkill(int slot)
    {
        if (PlayerManager.instance.UseSkill(slot))
            skillSlots[slot].interactable = false;
    }

    public override void ShowQuestRoot(EAchievementType type)
    {
        switch (type)
        {
            case EAchievementType.UseSkill:
                // foreach (var slot in skillSlots)
                // {
                //     if (slot.interactable)
                //     {
                //         questGuide.position = slot.transform.position;
                //         break;
                //     }
                // }
                for (int i = 0; i < 6; ++i)
                {
                    if (PlayerManager.instance.CanUseSkill(i))
                    {
                        questGuide.position = skillSlots[i].transform.position;
                        questGuide.gameObject.SetActive(true);
                        QuestManager.instance.currentQuest.onComplete += x => questGuide.gameObject.SetActive(false);
                        break;
                    }
                }
                break;
            case EAchievementType.UseAutoSkill:
                questGuide.position = autoSkill.transform.position;
                questGuide.gameObject.SetActive(true);
                QuestManager.instance.currentQuest.onComplete += x => questGuide.gameObject.SetActive(false);
                break;
        }
        
        
    }
}