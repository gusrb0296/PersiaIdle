using System;
using UnityEngine;

[Serializable]
public class StackAchievement : BaseAchievement
{
    public int goal;
    public int descriptionGoal;
    public EQuestRewardType rewardType;
    public int reward;

    public StackAchievement(string id, string title, string description, EAchievementType type, int goal, int descGoal, EQuestRewardType rewardType, int reward) : base(id, title, description, type)
    {
        this.goal = goal;
        this.rewardType = rewardType;
        this.reward = reward;
        this.descriptionGoal = descGoal;
    }
    
    public override void CompleteAchievement()
    {
        base.CompleteAchievement();
    }

    public override void InitializeInfo(QuestManager questManager)
    {
        base.InitializeInfo(questManager);
    }

    public override void GetReward()
    {
        base.GetReward();
    }

    public override int GetRewardAmount()
    {
        return reward;
    }

    public override EQuestRewardType GetRewardType()
    {
        return rewardType;
    }

    public override int GetGoal()
    {
        return goal;
    }

    public override int GetDescriptionGoal()
    {
        return descriptionGoal;
    }

    public override int GetCount()
    {
        return count - goal + descriptionGoal;
    }

    public override string GetDescription()
    {
        return description;
    }

    public override int GetID()
    {
        return base.GetID();
    }

    public override void Save()
    {
        base.Save();
    }

    public override void Load()
    {
        base.Load();
    }

    public override void InitCount()
    {
        base.InitCount();

        // 던전 진입, 소환에 관해서는 카운트를 하지 않는 것
        // if (type is (< EAchievementType.GoldDungeonPlayCount or >= EAchievementType.StatUpgradeCount)
        //     and (< EAchievementType.WeaponSummonCount or >= EAchievementType.ClearStageLevel)) return;
        if (type == EAchievementType.KillCount)
        {
            count = 0;
            return;
        }
        
        count = manager.GetCheckerCount(type);
        if (count >= GetGoal())
        {
            count = GetGoal();
            QuestManager.instance.GetChecker(type).Save();
            CompleteAchievement();
        }
    }
}
