using System;
using Defines;
using Keiwando.BigInteger;
using UnityEngine.Serialization;

public abstract class CurrentStatus
{
    public BigInteger currentAttack;
    public BigInteger currentMaxHealth;
    public float currentDamageReduction;
    public float currentCriticalChance;
    public BigInteger currentCriticalDamage;
    public float currentAttackSpeed;
    public float currentMovementSpeed;
    public BigInteger currentSkillDamage;
}

public class StatBase<T0, T1>
{
    private EStatusType type;
    
    private T0 baseData;
    private T0 perLevelData;

    private T0 addModifier;
    private T1 multiplyModifier;
    
    private T1 finalData;
    public event Action<T1> onChange;
}

[Serializable]
public class BaseStatus
{
    public int baseAttack;
    public int baseHealth;
    public float baseDamageReduction;
    public int baseMana;
    public int baseManaRecovery;
    public float baseCritChance;
    public int baseCritDamage;
    public float baseAttackSpeed;
    public float baseMovementSpeed;
    
    public static BaseStatus operator + (BaseStatus a, BaseStatus b)
    {
        BaseStatus ret = new BaseStatus
        {
            baseAttack = a.baseAttack + b.baseAttack,
            baseHealth = a.baseHealth + b.baseHealth,
            baseDamageReduction = a.baseDamageReduction + b.baseDamageReduction,
            baseMana = a.baseMana + b.baseMana,
            baseManaRecovery = a.baseManaRecovery + b.baseManaRecovery,
            baseCritChance = a.baseCritChance + b.baseCritChance,
            baseAttackSpeed = a.baseAttackSpeed + b.baseAttackSpeed,
            baseMovementSpeed = a.baseMovementSpeed + b.baseMovementSpeed
        };

        return ret;
    }
}

[Serializable]
public struct TempBuffStatus
{
    public int attackBuff;
    public int healthBuff;
    public float damageReductionBuff;
    public int manaBuff;
    public int manaRecoveryBuff;
    public float critChanceBuff;
    public int critDamageBuff;
    public float attackSpeedBuff;
    public float movementSpeedBuff;
    public int skillDamageBuff;

    public TempBuffStatus(int levelFrom1, TempBuffStatus status)
    {
        attackBuff = status.attackBuff * levelFrom1;
        healthBuff = status.healthBuff * levelFrom1;
        damageReductionBuff = status.damageReductionBuff * levelFrom1;
        manaBuff = status.manaBuff * levelFrom1;
        manaRecoveryBuff = status.manaRecoveryBuff * levelFrom1;
        critChanceBuff = status.critChanceBuff * levelFrom1;
        critDamageBuff = status.critDamageBuff * levelFrom1;
        attackSpeedBuff = status.attackSpeedBuff * levelFrom1;
        movementSpeedBuff = status.movementSpeedBuff * levelFrom1;
        skillDamageBuff = status.skillDamageBuff * levelFrom1;
    }

    public TempBuffStatus(int atk, int hp, int dmg_red, int mana, int mana_rec, int crit_ch,
        int crit_dmg, int atk_spd, int mov_spd, int skill)
    {
        attackBuff = atk;
        healthBuff = hp;
        damageReductionBuff = dmg_red / 100f;
        manaBuff = mana;
        manaRecoveryBuff = mana_rec;
        critChanceBuff = crit_ch / 100f;
        critDamageBuff = crit_dmg;
        attackSpeedBuff = atk_spd / 100f;
        movementSpeedBuff = mov_spd / 100f;
        skillDamageBuff = skill;
    }

    public static TempBuffStatus operator -(TempBuffStatus lh, TempBuffStatus rh)
    {
        TempBuffStatus ret;
        ret.attackBuff = lh.attackBuff - rh.attackBuff;
        ret.healthBuff = lh.healthBuff - rh.healthBuff;
        ret.damageReductionBuff = lh.damageReductionBuff - rh.damageReductionBuff;
        ret.manaBuff = lh.manaBuff - rh.manaBuff;
        ret.manaRecoveryBuff = lh.manaRecoveryBuff - rh.manaRecoveryBuff;
        ret.critChanceBuff = lh.critChanceBuff - rh.critChanceBuff;
        ret.critDamageBuff = lh.critDamageBuff - rh.critDamageBuff;
        ret.attackSpeedBuff = lh.attackSpeedBuff - rh.attackSpeedBuff;
        ret.movementSpeedBuff = lh.movementSpeedBuff - rh.movementSpeedBuff;
        ret.skillDamageBuff = lh.skillDamageBuff - rh.skillDamageBuff;
        
        return ret;
    }

    public static TempBuffStatus operator +(TempBuffStatus lh, TempBuffStatus rh)
    {
        TempBuffStatus ret;
        ret.attackBuff = lh.attackBuff + rh.attackBuff;
        ret.healthBuff = lh.healthBuff + rh.healthBuff;
        ret.damageReductionBuff = lh.damageReductionBuff + rh.damageReductionBuff;
        ret.manaBuff = lh.manaBuff + rh.manaBuff;
        ret.manaRecoveryBuff = lh.manaRecoveryBuff + rh.manaRecoveryBuff;
        ret.critChanceBuff = lh.critChanceBuff + rh.critChanceBuff;
        ret.critDamageBuff = lh.critDamageBuff + rh.critDamageBuff;
        ret.attackSpeedBuff = lh.attackSpeedBuff + rh.attackSpeedBuff;
        ret.movementSpeedBuff = lh.movementSpeedBuff + rh.movementSpeedBuff;
        ret.skillDamageBuff = lh.skillDamageBuff + rh.skillDamageBuff;
        
        return ret;
    }
}

[Serializable]
public struct PassiveStatus
{
    public EStatusType target;
    public int buff;

    public PassiveStatus(EStatusType status, int buffAmount)
    {
        target = status;
        buff = buffAmount;
    }
}

[Serializable]
public class BaseAwaken
{
    public float baseAttackAmp;
    public float baseHealthAmp;

    public float baseAttackSpeedAmp;
    public float baseMovementSpeedAmp;

    public float baseSkillDamageAmp;
    public float baseBossDamageAmp;

    public float baseSpecialAttackAmp;
    public float baseSpecialDefenceAmp;
    public float baseSpecialDurationAmp;
    public float baseSpecialAttackSpeedAmp;
}

[Serializable]
public class BaseSpeciality
{
    public int baseAttackMin;
    public int baseAttackMax;

    public int baseHealthMin;
    public int baseHealthMax;

    public float baseCritChAmpMin;
    public float baseCritChAmpMax;

    public float baseCirtDmgAmpMin;
    public float baseCirtDmgAmpMax;

    public float baseOverallDamageAmpMin;
    public float baseOverallDamageAmpMax;

    public float baseSkillDamageAmpMin;
    public float baseSkillDamageAmpMax;

    public float baseNormalDamageAmpMin;
    public float baseNormalDamageAmpMax;

    public float baseBossDamageAmpMin;
    public float baseBossDamageAmpMax;

    public float baseHealthAmpMin;
    public float baseHealthAmpMax;
}

[Serializable]
public class BaseRelic
{
    public float baseAttackAmp;
    public float baseHealthAmp;
    public float baseDefenceAmp;
    public float baseManaAmp;

    public float baseCritChAmp;
    public float baseCritDmgAmp;

    public float baseSkillDamageAmp;
    public float baseBossDamageAmp;

    public float baseGoldEarnAmp;
    public float baseExpEarnAmp;
}

public interface IBuffTarget
{
    public void AddBuffToList(TempBuffStatus tempBuff);
    public void RemoveBuffFromList(TempBuffStatus tempBuff);
}

