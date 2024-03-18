using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "SO/ActiveSkillFixedInfo", fileName = "ActiveSkillFixedInfo")]
public class ActiveSkillFixedInfo : FixedInfo
{
    [Header("Animation")]
    public EFsmState animType;
    public string animParameter;
    public float skillAnimTime;
    
    [Header("Attack")]
    public ESkillAttackType attackType;
    public bool isFollowing = false;
    public bool isContinuous = false;
    public bool isRepeat = false;
    public float attackDistance;
    public AttackColliderInfo[] attackColliderInfos;
}
