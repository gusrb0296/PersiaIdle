using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAchievementElement : UIBase
{
    public TMP_Text title;
    public TMP_Text description;
    public TMP_Text count;
    public Slider slider;
    public Button complete;
    private BaseAchievement achievement;
    
    protected void Awake()
    {
        complete.onClick.AddListener(OnCompleteBtn);
    }

    public void ShowUI(BaseAchievement target)
    {
        base.ShowUI();
        
        achievement = target;
        UpdateDisplay(target);
        
        // TODO disable button cleared achievement

        achievement.onUpdateCounter += UpdateCounter;
    }

    public override void CloseUI()
    {
        base.CloseUI();
        
        achievement.onUpdateCounter -= UpdateCounter;
    }

    private void UpdateDisplay(BaseAchievement target)
    {
        title.text = target.title;
        description.text = target.GetDescription();
        slider.maxValue = target.GetGoal();
        
        UpdateCounter(0);
    }

    private void UpdateCounter(int notUse)
    {
        count.text = $"{achievement.GetCount()} / {achievement.GetGoal()}";
        slider.value = achievement.GetCount();
        
        if (achievement.isComplete)
            complete.interactable = true;
        else
        {
            complete.interactable = false;
        }
    }

    private void OnCompleteBtn()
    {
        achievement.GetReward();
        achievement.Save();
        UpdateDisplay(achievement);
    }
}
