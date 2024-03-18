using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "SO/SkillSummonGacha", fileName = "SkillSummonGacha")]
public class SkillSummonGacha : ScriptableObject
{
    public int[] weightPerRarities;
    private int totalWeight;

    public virtual BaseSkillData Summon()
    {
        InitWeight();
        var ran = Random.Range(1,totalWeight+1);

        int current = 0;
        for (int i = 0; i < weightPerRarities.Length; ++i)
        {
            current += weightPerRarities[i];
            if (current >= ran)
            {
                var skills = SkillManager.instance.GetSkillsOnRarity((ERarity)i);
                Debug.Assert(skills.Count > 0, $"{(ERarity)i}등급의 스킬이 부족합니다.");
                var index = Random.Range(0, skills.Count);
                return skills[index];
            } 
        }

        // 그럴 일은 없겠지만 끝까지 간 경우에 대한 예외처리
        Debug.Assert(false, "랜덤값이 이상합니다.");
        return null;
    }

    public virtual void InitWeight()
    {
        int ret = 0;
        foreach (int weightPerRarity in weightPerRarities)
        {
            ret += weightPerRarity;
        }

        totalWeight = ret;
    }

    public virtual float GetPercentage(ERarity rarity)
    {
        float weight = weightPerRarities[(int)rarity];
        float percentage = weight / totalWeight;
        return percentage;
    }
}
