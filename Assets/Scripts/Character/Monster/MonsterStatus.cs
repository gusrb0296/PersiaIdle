using System;
using Keiwando.BigInteger;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class MonsterStatus : CurrentStatus
{
    // TODO debuff를 적용할 수 있도록 구조를 바꿀 것.
    public MonsterDropData[] rewards;
    public int level;
    public void InitStatus(BaseStatus baseStatus, BaseStatus perLevel, int level, MonsterDropData[] monsterReward)
    {
        currentAttack = (BigInteger)baseStatus.baseAttack + (BigInteger)perLevel.baseAttack * level;
        currentMaxHealth = (BigInteger)baseStatus.baseHealth + (BigInteger)perLevel.baseHealth * level;
        // currentHealth = (BigInteger)baseStatus.baseHealth + (BigInteger)perLevel.baseHealth * level;
        currentDamageReduction = baseStatus.baseDamageReduction + perLevel.baseDamageReduction * level;
        currentCriticalChance = baseStatus.baseCritChance + perLevel.baseCritChance * level;
        currentCriticalDamage = (BigInteger)baseStatus.baseCritDamage + (BigInteger)perLevel.baseCritDamage * level;
        currentAttackSpeed = baseStatus.baseAttackSpeed + perLevel.baseAttackSpeed * level;
        // currentAttackRange = baseStatus.baseAttackRange + perLevel.baseAttackRange * level;
        currentMovementSpeed = baseStatus.baseMovementSpeed + perLevel.baseMovementSpeed * level;
        currentSkillDamage = 100;
        this.level = level;

        rewards = monsterReward;
        foreach (var reward in rewards)
        {
            if (reward.currentRewardAmount <= 0)
            {
                reward.InitCurrentReward();
            }
        }
    }

    public void GetReward(int index, out EQuestRewardType type, out BigInteger amount)
    {
        type = rewards[index].rewardType;
        amount = rewards[index].currentRewardAmount;
    }
}