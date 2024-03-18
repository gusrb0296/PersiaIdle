using System;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class RepeatAchievement : BaseAchievement
{
    public int levelFrom0 { get; protected set; }

    protected int stateIndex;
    public int[] levelLimit;
    public int[] goal;
    public EQuestRewardType rewardType;
    public int[] rewardAmount;
    protected int totalCountGoal;
    protected string[] descriptions;

    public RepeatAchievement(int[] level, int[] goal, EQuestRewardType rewardType, int[] rewardAmount, string id,
        string title, string description, EAchievementType type) : base(id, title, description, type)
    {
        levelLimit = new int[level.Length];
        Array.Copy(level, levelLimit, levelLimit.Length);
        this.goal = new int[goal.Length];
        Array.Copy(goal, this.goal, this.goal.Length);
        this.rewardType = rewardType;
        this.rewardAmount = new int[rewardAmount.Length];
        Array.Copy(rewardAmount, this.rewardAmount, this.rewardAmount.Length);
        stateIndex = 0;
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

        isComplete = false;
        isRewarded = false;

        ++levelFrom0;
        if (levelFrom0 >= levelLimit[stateIndex])
        {
            stateIndex = Mathf.Min(stateIndex + 1, levelLimit.Length - 1);
        }

        if (type == EAchievementType.KillCount)
            totalCountGoal = goal[stateIndex];
        else
            totalCountGoal += goal[stateIndex];

        // manager.SubscribeCounter(this);
        //
        // count = manager.GetCheckerCount(type);
        // if (count >= GetGoal())
        // {
        //     count = GetGoal();
        //     CompleteAchievement();
        // }
    }

    public override int GetRewardAmount()
    {
        return rewardAmount[stateIndex];
    }

    public override EQuestRewardType GetRewardType()
    {
        return rewardType;
    }

    public override int GetGoal()
    {
        return totalCountGoal;
    }

    public override int GetDescriptionGoal()
    {
        if (QuestManager.repeatQuestDescriptionTypeA.Contains(type))
            return goal[stateIndex];
        if (QuestManager.repeatQuestDescriptionTypeB.Contains(type))
            return totalCountGoal;

        return 0;
    }

    public override int GetID()
    {
        var questNum = base.GetID();
        var ret = questNum + QuestManager.instance.quests.Length + levelFrom0 * QuestManager.instance.repeatQuest.Length;
        return ret;
    }
    public override int GetCount()
    {
        if (type == EAchievementType.KillCount)
            return count;
        if (QuestManager.repeatQuestDescriptionTypeA.Contains(type))
            return count - totalCountGoal + goal[stateIndex];
        // if (QuestManager.repeatQuestDescriptionTypeB.Contains(type))
        return count;
    }

    public override string GetDescription()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(descriptions[0]);
        if (type == EAchievementType.ClearStageLevel)
        {
            sb.Append(totalCountGoal / 20 + 1);
            sb.Append("-");
            int stage = (totalCountGoal-1) % 20;
            if (stage == 0) stage = 20;
            sb.Append(stage);
        }
        else if (QuestManager.repeatQuestDescriptionTypeA.Contains(type))
        {
            sb.Append(goal[stateIndex]);
        }
        else if (QuestManager.repeatQuestDescriptionTypeB.Contains(type))
        {
            sb.Append(totalCountGoal);
        }

        sb.Append(descriptions[1]);

        return sb.ToString();
    }

    public override void Save()
    {
        // base.Save();
        DataManager.Instance.Save($"{achievementID}_{nameof(levelFrom0)}", levelFrom0);
    }

    public override void Load()
    {
        levelFrom0 = DataManager.Instance.Load($"{achievementID}_{nameof(levelFrom0)}", 0);
        // isRewarded = DataManager.Instance.Load($"{achievementID}_{nameof(isRewarded)}", false);
        stateIndex = 0;
        totalCountGoal = goal[0];

        for (int i = 0; i <= levelFrom0; ++i)
        {
            if (i >= levelLimit[stateIndex])
                ++stateIndex;

            if (type == EAchievementType.KillCount)
                totalCountGoal = goal[stateIndex];
            else
                totalCountGoal += goal[stateIndex];
        }

        descriptions = description.Split('*');

        if (type == EAchievementType.KillCount) return;
        
        count = manager.GetCheckerCount(type);
        if (count >= GetGoal())
        {
            count = GetGoal();
            CompleteAchievement();
        }
    }

    public override void InitCount()
    {
        base.InitCount();

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