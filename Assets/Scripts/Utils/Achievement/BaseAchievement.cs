using System;
using UnityEngine;
[Serializable]
public class BaseAchievement
{
    public string achievementID;

    public string title;
    public string description;

    public EAchievementType type;

    
    #region Field for Runtime

    public int count;
    public bool isComplete;
    public bool isRewarded;
    protected QuestManager manager;
    public event Action<BaseAchievement> onStart;
    public event Action<BaseAchievement> onComplete;
    public event Action<EQuestRewardType, int> onRewarded;
    public event Action<int> onUpdateCounter;
    
    #endregion

    public BaseAchievement(string id, string title, string description, EAchievementType type)
    {
        achievementID = id;
        this.title = title;
        this.description = description;
        this.type = type;
    }
    
    public virtual void UpdateCounter(int current)
    {
        count = current > GetGoal() ? GetGoal() : current;
        onUpdateCounter?.Invoke(current);
        if (count >= GetGoal())
        {
            QuestManager.instance.GetChecker(type).Save();
            CompleteAchievement();
        }
    }

    public virtual void CompleteAchievement()
    {
        onUpdateCounter = null;
        onComplete?.Invoke(this);
        isComplete = true;
        manager.UnsubscribeCounter(this);
    }

    public virtual void InitializeInfo(QuestManager questManager)
    {
        manager = questManager;
        // isComplete = false;
        // isRewarded = false;
    }

    public virtual void GetReward()
    {
        if (manager.GiveReward(GetRewardType(), GetRewardAmount()))
        {
            onRewarded?.Invoke(GetRewardType(), GetRewardAmount());
            isRewarded = true;
        }
    }

    public virtual int GetRewardAmount()
    {
        return 0;
    }

    public virtual EQuestRewardType GetRewardType()
    {
        return EQuestRewardType.Gold;
    }

    public virtual int GetGoal()
    {
        return 1;
    }

    public virtual int GetDescriptionGoal()
    {
        return 1;
    }

    public virtual int GetCount()
    {
        return count;
    }

    public virtual string GetDescription()
    {
        return description;
    }

    public virtual int GetID()
    {
        var id = achievementID.Split(' ');
        var num = int.Parse(id[^1]);
        return num;
    }

    public virtual void Save()
    {
        DataManager.Instance.Save($"{achievementID}_{nameof(isRewarded)}", isRewarded);
    }

    public virtual void Load()
    {
        isRewarded = DataManager.Instance.Load($"{achievementID}_{nameof(isRewarded)}", false);
        isComplete = isRewarded;
    }

    public virtual void InitCount()
    {
        onStart?.Invoke(this);
    }
}