using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using Unity.Collections;
using UnityEngine;


[Serializable]
public class BaseLevelSystem
{
    public event Action<int> onLevelChange;
    public event Action<BigInteger, BigInteger> onEarnEXP;
    
    public BigInteger ExpCap => expCap;
    public BigInteger Exp => currentExp;
    public int Level => currentLevel;

    protected BigInteger expCap = 10;
    protected int level = 1;
    
    protected BigInteger currentExp;
    protected int currentLevel;

    [SerializeField] protected int baseExp;
    [SerializeField] protected int maxLevel;

    // 해당 레벨에 대한 필요 경험치를 계산하고, 소지 경험치와 레벨을 세팅해준다.
    public void InitSystem(int _level, BigInteger _exp)
    {
        expCap = GetRequiredExp(_level);
        level = _level;

        currentLevel = level;
        currentExp = _exp;
    }
    
    // 경험치를 얻고, 필요 경험치를 넘어간 경우에 레벨업을 한다.
    public virtual void EarnExp(int earn)
    {
        currentExp += earn;
        // Debug.Log($"Earn {currentExp}/{exp}exps");
        while (currentExp >= expCap)
        {
            LevelUp();
        }
        onEarnEXP?.Invoke(currentExp, expCap);
    }

    public virtual void EarnExp(BigInteger earn)
    {
        currentExp += earn;
        // Debug.Log($"Earn {currentExp}/{exp}exps");
        while (currentExp >= expCap)
        {
            LevelUp();
        }
        onEarnEXP?.Invoke(currentExp, expCap);
    }
    
    // 다음 레벨에 필요한 경험치를 계산한다. 현재 경험치을 이용하기에 가볍다.
    protected virtual BigInteger GetNextRequiredExp(BigInteger _exp)
    {
        _exp += _exp / 5;
        return _exp;
    }

    // 해당 레벨에 필요 경험치를 계산하다. 거듭제곱을 사용하기에 무겁다.
    public virtual BigInteger GetRequiredExp(int _level)
    {
        return baseExp * Convert.ToInt32(Mathf.Pow(1.2f,_level-1));
    }

    // 레벨업을 하며, 등록된 관찰자들을 호출한다.
    public virtual void LevelUp()
    {
        if (maxLevel <= level)
            return;
        
        currentExp -= expCap;
        expCap = GetNextRequiredExp(expCap);
        
        level++;
        currentLevel = level;

        // SaveLevelExp(PlayerManager.instance.userName);
        
        onLevelChange?.Invoke(level);
        // Debug.Log($"Level UP!! {level-1}=>{level}");
    }

    public virtual void SaveLevelExp(string id)
    {
        DataManager.Instance.Save<int>($"{id}_{nameof(currentLevel)}", currentLevel);
        DataManager.Instance.Save<string>($"{id}_Í{nameof(currentExp)}", currentExp.ToString());
    }

    public virtual void LoadLevelExp(string id)
    {
        var lv = DataManager.Instance.Load<int>($"{id}_{nameof(currentLevel)}", 1);
        var ex = new BigInteger(DataManager.Instance.Load<string>($"{id}_{nameof(currentExp)}", "0"));
        
        InitSystem(lv, ex);
    }
}
