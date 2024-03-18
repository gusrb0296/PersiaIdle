using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "SO/BuffSkillFixedInfo", fileName = "BuffSkillFixedInfo")]
public class BuffSkillFixedInfo : FixedInfo
{
    [Header("Animation")]
    public EFsmState animType;
    public string animParameter;
    public float skillAnimTime;
}
