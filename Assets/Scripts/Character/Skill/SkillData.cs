using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class BaseSkillData
{
    [Header("필수")] public string skillName;
    public ERarity rarity;
    public string description;
    public string[] descriptions { get; protected set; }
    public ESkillType skillType;
    public int iconIndex;

    [Header("확인용")] public bool isOwned;
    public int quantity;
    public int levelFrom0;

    public event Action<int> onLevelUp;

    public BaseSkillData(string _skillName, ERarity _rarity, string _description, ESkillType _skillType)
    {
        skillName = _skillName;
        rarity = _rarity;
        description = _description;
        skillType = _skillType;
        isOwned = false;
        quantity = 0;
        levelFrom0 = 0;
    }

    public virtual void Save(ESkillDataType type)
    {
        switch (type)
        {
            case ESkillDataType.Quantity:
                DataManager.Instance.Save<int>(skillName + nameof(quantity), quantity);
                break;
            case ESkillDataType.Level:
                DataManager.Instance.Save<int>(skillName + nameof(levelFrom0), levelFrom0);
                break;
        }
    }

    public virtual void Load()
    {
        descriptions = description.Split('n', 'm');
        quantity = DataManager.Instance.Load(skillName + nameof(quantity), 0);
        isOwned = quantity > 0;
        if (isOwned)
        {
            levelFrom0 = DataManager.Instance.Load(skillName + nameof(levelFrom0), 0);
        }
        else
            levelFrom0 = 0;
    }

    public virtual string GetDescriptionVariable(int index)
    {
        return "0";
    }

    public virtual bool TryLevelUp()
    {
        if (IsCanLevelUp())
        {
            quantity -= 4 * (levelFrom0 + 1);
            ++levelFrom0;
            onLevelUp?.Invoke(levelFrom0);
            return true;
        }

        return false;
    }

    public bool IsCanLevelUp()
    {
        return quantity > 4 * (levelFrom0 + 1);
    }

    public int QuantityToLevelUp()
    {
        return 4 * (levelFrom0 + 1);
    }
}

[Serializable]
public abstract class AnimSkillData : BaseSkillData
{
    [Header("시전")] public float coolTime;
    public int ManaConsume { get; protected set; }
    [SerializeField] protected int baseManaConsume;
    public float skillFullTime;

    [Header("애니메이션")] public EFsmState animType;
    public string animParameter;
    public float skillAnimTime;
    
    public bool isEquipped { get; set; }
    public int equipIndex;

    public AnimSkillData(string _skillName, ERarity _rarity, string _description, ESkillType _skillType,
        float _cooltime, int _manaconsume, float _skillFullTime) : base(_skillName, _rarity, _description, _skillType)
    {
        coolTime = _cooltime;
        baseManaConsume = _manaconsume;
        ManaConsume = _manaconsume * (levelFrom0 + 1);
        skillFullTime = _skillFullTime;
    }

    public override void Save(ESkillDataType type)
    {
        if (type == ESkillDataType.EquipIndex)
        {
            DataManager.Instance.Save<int>(skillName + nameof(equipIndex), equipIndex);
        }
        else
        {
            base.Save(type);
        }
    }

    public override void Load()
    {
        base.Load();
        ManaConsume = baseManaConsume * (levelFrom0 + 1);

        if (isOwned)
        {
            equipIndex = DataManager.Instance.Load(skillName + nameof(equipIndex), -1);

            if (equipIndex != -1)
            {
                // TODO : not good
                PlayerManager.instance.EquipSkill(equipIndex, this);
                UIManager.instance.TryGetUI<UISkillSlot>().ShowUI(equipIndex, this);
            }
        }
    }

    public void SaveEquip(bool isEquip, int index = -1)
    {
        isEquipped = isEquip;
        equipIndex = index;
        Save(ESkillDataType.EquipIndex);
    }

    public override bool TryLevelUp()
    {
        if (base.TryLevelUp())
        {
            ManaConsume = baseManaConsume * (levelFrom0 + 1);
            return true;
        }

        return false;
    }

    public abstract bool TryGetShakeTime(int index, out float time);
    public abstract float[] GetShakeTimes();
}

[Serializable]
public class ActiveSkillData : AnimSkillData
{
    public ESkillAttackType attackType { get; set; }
    public bool isFollowing { get; set; } = false;
    public bool isContinuous { get; set; } = false;
    public bool isRepeat { get; set; } = false;
    public AttackColliderInfo[] colliderInfo { get; set; }

    [Header("Attack")] public int maxAttackCount = 1;
    public int Multiplier { get; protected set; }
    [SerializeField] protected int baseMultiplier;
    public float attackDistance;
    public float tickUnitTime;

    public ActiveSkillData(string _skillName, ERarity _rarity, string _description,
        ESkillType _skillType, float _cooltime, int _manaconsume, float _skillFullTime,
        int _maxAttackCount, int multiplier, float _attackDistance, float _tickUnitTime)
        : base(_skillName, _rarity, _description, _skillType, _cooltime, _manaconsume, _skillFullTime)
    {
        maxAttackCount = _maxAttackCount;
        baseMultiplier = multiplier;
        Multiplier = baseMultiplier * (1 + levelFrom0);
        attackDistance = _attackDistance;
        tickUnitTime = _tickUnitTime;
    }

    public void SetInfo(ActiveSkillFixedInfo info)
    {
        animType = info.animType;
        animParameter = info.animParameter;
        skillAnimTime = info.skillAnimTime;
        iconIndex = info.iconIndex;
        attackType = info.attackType;
        isFollowing = info.isFollowing;
        isContinuous = info.isContinuous;
        isRepeat = info.isRepeat;
        attackDistance = info.attackDistance;
        colliderInfo = info.attackColliderInfos;
    }

    public override string GetDescriptionVariable(int index)
    {
        if (index == 0)
            return Multiplier.ToString();
        else if (index == 1)
            return coolTime.ToString("F1");
        return "";
    }

    public override void Load()
    {
        base.Load();
        Multiplier = baseMultiplier * (levelFrom0 + 1);
    }

    public override bool TryLevelUp()
    {
        if (base.TryLevelUp())
        {
            Multiplier = baseMultiplier * (levelFrom0 + 1);
            // TODO : not good
            SkillManager.instance.GetSkillSystem(skillName).InitSkillSystem(PlayerManager.instance.player, this);
            return true;
        }

        return false;
    }

    public override bool TryGetShakeTime(int index, out float time)
    {
        if (index < colliderInfo.Length)
        {
            time = colliderInfo[index].shakeTime;
            return true;
        }

        time = float.MaxValue;
        return false;
    }

    public override float[] GetShakeTimes()
    {
        float[] ret = new float[colliderInfo.Length];
        
        for (int i = 0; i < ret.Length; ++i)
            ret[i] = colliderInfo[i].shakeTime;

        return ret;
    }
}

[Serializable]
public class PassiveSkillData : BaseSkillData
{
    [Header("Passive Status")] [SerializeField]
    protected PassiveStatus baseStatus;

    public PassiveStatus status { get; protected set; }

    public PassiveSkillData(string skillName, ERarity rarity, string description, ESkillType skillType,
        EStatusType type, int amount) : base(skillName, rarity, description, skillType)
    {
        baseStatus = new PassiveStatus(type, amount);
        status = new PassiveStatus(baseStatus.target, baseStatus.buff * (1 + levelFrom0));
    }

    public void SetInfo(PassiveSkillFixedInfo info)
    {
        iconIndex = info.iconIndex;
    }

    public override string GetDescriptionVariable(int index)
    {
        return status.buff.ToString();
    }

    public override void Load()
    {
        base.Load();
        status = new PassiveStatus(baseStatus.target, baseStatus.buff * (1 + levelFrom0));

        if (isOwned)
        {
            // TODO : not good
            PlayerManager.instance.AddPassiveToList(status);
        }
    }

    public override bool TryLevelUp()
    {
        if (base.TryLevelUp())
        {
            // TODO : not good
            PlayerManager.instance.RemovePassiveToList(status);
            status = new PassiveStatus(baseStatus.target, baseStatus.buff * (1 + levelFrom0));
            PlayerManager.instance.AddPassiveToList(status);
            return true;
        }

        return false;
    }
}

[Serializable]
public class BuffSkillData : AnimSkillData
{
    public TempBuffStatus tempBuffStatus { get; protected set; }

    [Header("Buff Status")]
    [SerializeField] protected TempBuffStatus baseBuffStatus;

    [SerializeField] protected float[] shakeTime;

    protected List<string> buffDescription;

    public BuffSkillData(string _skillName, ERarity _rarity, string _description, ESkillType _skillType,
        float _cooltime, int _manaconsume, float _skillFullTime, TempBuffStatus _status)
        : base(_skillName, _rarity, _description, _skillType, _cooltime, _manaconsume, _skillFullTime)
    {
        baseBuffStatus = _status;
        tempBuffStatus = new TempBuffStatus(levelFrom0 + 1, baseBuffStatus);
    }

    public void SetInfo(BuffSkillFixedInfo info)
    {
        animType = info.animType;
        animParameter = info.animParameter;
        skillAnimTime = info.skillAnimTime;
        iconIndex = info.iconIndex;
    }

    public override string GetDescriptionVariable(int index)
    {
        return buffDescription[index];
    }

    public override void Load()
    {
        base.Load();
        tempBuffStatus = new TempBuffStatus(levelFrom0 + 1, baseBuffStatus);

        buffDescription = new List<string>();
        if (tempBuffStatus.attackBuff > 0)
            buffDescription.Add(tempBuffStatus.attackBuff.ToString());
        if (tempBuffStatus.healthBuff > 0)
            buffDescription.Add(tempBuffStatus.healthBuff.ToString());
        if (tempBuffStatus.damageReductionBuff > 0)
            buffDescription.Add((tempBuffStatus.damageReductionBuff * 100).ToString("N0"));
        if (tempBuffStatus.manaBuff > 0)
            buffDescription.Add(tempBuffStatus.manaBuff.ToString());
        if (tempBuffStatus.manaRecoveryBuff > 0)
            buffDescription.Add(tempBuffStatus.manaRecoveryBuff.ToString());
        if (tempBuffStatus.critChanceBuff > 0)
            buffDescription.Add((tempBuffStatus.critChanceBuff * 100).ToString("N0"));
        if (tempBuffStatus.critDamageBuff > 0)
            buffDescription.Add(tempBuffStatus.critDamageBuff.ToString());
        if (tempBuffStatus.attackSpeedBuff > 0)
            buffDescription.Add((tempBuffStatus.attackSpeedBuff * 100).ToString("N0"));
        if (tempBuffStatus.movementSpeedBuff > 0)
            buffDescription.Add((tempBuffStatus.movementSpeedBuff * 100).ToString("N0"));
        if (tempBuffStatus.skillDamageBuff > 0)
            buffDescription.Add(tempBuffStatus.skillDamageBuff.ToString());
    }

    public override bool TryLevelUp()
    {
        if (base.TryLevelUp())
        {
            tempBuffStatus = new TempBuffStatus(levelFrom0 + 1, baseBuffStatus);
            return true;
        }

        return false;
    }

    public override bool TryGetShakeTime(int index, out float time)
    {
        if (index < shakeTime.Length)
        {
            time = shakeTime[index];
            return true;
        }

        time = float.MaxValue;
        return false;
    }

    public override float[] GetShakeTimes()
    {
        return shakeTime;
    }
}

[Serializable]
public class SpecialSkillData : ActiveSkillData
{
    [Header("Buff Status")] public TempBuffStatus tempBuffStatus;

    public SpecialSkillData(string _skillName, ERarity _rarity, string _description, ESkillType _skillType,
        float _cooltime, int _manaconsume, float _skillFullTime, int _maxAttackCount, int multiplier,
        float _attackDistance, float _tickUnitTime, TempBuffStatus buff) : base(_skillName, _rarity, _description,
        _skillType, _cooltime, _manaconsume, _skillFullTime, _maxAttackCount, multiplier, _attackDistance,
        _tickUnitTime)
    {
        tempBuffStatus = buff;
    }
}

[Serializable]
public class AttackColliderInfo
{
    public ECalculatePositionType type;
    public float size;
    public int knockback;
    public float startTime;
    public float duration;
    public Vector3 offset;
    public float shakeTime;
}