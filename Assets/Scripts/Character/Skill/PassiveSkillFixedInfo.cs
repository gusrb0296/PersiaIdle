using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "SO/PassiveSkillFixedInfo", fileName = "PassiveSkillFixedInfo")]
public class PassiveSkillFixedInfo : FixedInfo
{
    
}

[Serializable]
public class FixedInfo : ScriptableObject
{
    [Header("필수")]
    public string skillName;
    public int iconIndex;
}