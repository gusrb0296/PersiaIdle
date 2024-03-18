using System;
using System.Collections.Generic;

[Serializable]
public class LevelAchievement : BaseAchievement
{
    public int level { get; protected set; }
    
    public List<int> goalPerLevel { get; protected set; }
    public List<EQuestRewardType> rewardTypePerLevel { get; protected set; }
    public List<int> rewardPerLevel { get; protected set; }
    
    public LevelAchievement(string id, string title, string description, EAchievementType type, int[] goals, EQuestRewardType[] rewardTypes, int[] rewards) : base(id, title, description, type)
    {
        goalPerLevel = new List<int>();
        goalPerLevel.AddRange(goals);
        rewardTypePerLevel = new List<EQuestRewardType>();
        rewardTypePerLevel.AddRange(rewardTypes);
        rewardPerLevel = new List<int>();
        rewardPerLevel.AddRange(rewards);
    }
    
    public override void UpdateCounter(int change)
    {
        base.UpdateCounter(change);
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

        ++level;
        isComplete = false;
        isRewarded = false;
        
        manager.SubscribeCounter(this);
        count = manager.GetCheckerCount(type);
        if (count >= goalPerLevel[level])
        {
            count = goalPerLevel[level];
            CompleteAchievement();
        }
    }

    public override int GetRewardAmount()
    {
        return rewardPerLevel[level];
    }

    public override EQuestRewardType GetRewardType()
    {
        return rewardTypePerLevel[level];
    }

    public override int GetGoal()
    {
        return goalPerLevel[level];
    }

    public override int GetCount()
    {
        return base.GetCount();
    }

    public override string GetDescription()
    {
        return goalPerLevel[level].ToString() + description;
    }

    public override void Save()
    {
        base.Save();
        DataManager.Instance.Save($"{achievementID}_{nameof(level)}", level);
    }

    public override void Load()
    {
        level = DataManager.Instance.Load($"{achievementID}_{nameof(level)}", 0);
        isRewarded = DataManager.Instance.Load($"{achievementID}_{nameof(isRewarded)}", false);
    }
}