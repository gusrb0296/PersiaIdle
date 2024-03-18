using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utils;

public class UIOffLineReward : UIBase
{
    [SerializeField] Button confirmBtn;

    [SerializeField] TMP_Text timeSpanText;
    [SerializeField] Slider timeSpanSlider;
    [SerializeField] TMP_Text monsterCountText;
    [SerializeField] Transform root;
    [SerializeField] private UIRewardElement prefab;
    private CustomPool<UIRewardElement> rewardPool;

    private List<Reward> rewards;

    public void Initialize()
    {
        rewardPool = EasyUIPooling.MakePool(prefab, root,
            null, null, null, 6, true);
        AddCallbacks();
    }

    private void AddCallbacks()
    {
        confirmBtn.onClick.AddListener(GiveReward);
    }

    public void ShowUI(int killCount, int timePasssed)
    {
        base.ShowUI();
        
        int hour = timePasssed / 3600;
        int minute = (timePasssed % 3600) / 60;
        int second = ((timePasssed % 3600) % 60);

        string time = "";
        if (hour > 0) time += $"{hour}시간 ";
        if (minute > 0) time += $"{minute}분 ";
        if (second > 0) time += $"{second}초";

        timeSpanText.text = time;
        timeSpanSlider.maxValue = 28800;
        timeSpanSlider.value = timePasssed;

        monsterCountText.text = $"{killCount}";

        SetReward(killCount);
        SetSlots();
    }

    private void SetReward(int killCount)
    {
        rewards = StageManager.instance.GetCurrentReward(killCount);
    }

    private void SetSlots()
    {
        foreach(Reward reward in rewards)
        {
            // TODO : Show Rewards
            if (reward.amount == 0) return;
            UIRewardElement slot = rewardPool.Get();
            slot.ShowUI(reward);
            // RewardBaseSO data = rewardManager.GetRewardBaseData(reward.rewardType);
            // RewardSlot slot = data.GetRewardSlot(reward);
            // slot.transform.SetParent(root);
        }
    }

    private void GiveReward()
    {
        PushNotificationManager.instance.GiveReward(rewards);
        CloseUI();
    }

    public override void CloseUI()
    {
        base.CloseUI();
        gameObject.SetActive(false);
        
        foreach (var ui in rewardPool.UsedList)
            ui.CloseUI();
        
        rewardPool.Clear();
    }

    public void AddReward(Reward reward)
    {
        if (reward.amount == 0) return;
        UIRewardElement slot = rewardPool.Get();
        slot.ShowUI(reward);
    }
}
