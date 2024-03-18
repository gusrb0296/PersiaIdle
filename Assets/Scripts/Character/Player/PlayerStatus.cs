using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using Keiwando.BigInteger;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class PlayerStatus : CurrentStatus, IBuffTarget
{
#if UNITY_EDITOR
    [Header("확인용")]
    public string testAttack;
    public string testHealth;
    public string testMana;
    public string testManaRecovery;
    public string testCriticalDamage;
    public string testSkillDamage;
#endif
    [Header("설정 필요")]
    // TODO 몬스터와 구조를 어느 정도 통일해서 버프, 디버프가 동일한 방법으로 적용되도록 구조 수정 필요.
    public BaseStatus level1Status;

    public BaseStatus perLevelStatus;
    public int Level => PlayerManager.instance.levelSystem.Level;

    protected BigInteger baseAttack;
    protected BigInteger attackPercent;
    public event Action<BigInteger> onAttackChange;

    protected BigInteger baseMaxHealth;
    protected BigInteger maxHealthPercent;
    public event Action<BigInteger> onMaxHealthChange;

    private float baseDamageReduction;
    protected float damageReductionPercent;
    public event Action<float> onDamageReductionChange;

    public BigInteger currentMaxMana;
    private BigInteger baseMaxMana;
    protected BigInteger maxManaPercent;
    public event Action<BigInteger> onMaxManaChange;

    public BigInteger currentManaRecovery;
    private BigInteger baseManaRecovery;
    protected BigInteger manaRecoveryPercent;
    public event Action<BigInteger> onManaRecoveryChange;

    private float baseCriticalChance;
    protected float criticalChancePercent;
    public event Action<float> onCriticalChanceChange;

    private BigInteger baseCriticalDamage;
    protected BigInteger criticalDamagePercent;
    public event Action<BigInteger> onCriticalDamageChange;

    private float baseAttackSpeed;
    protected float attackSpeedPercent;
    public event Action<float> onAttackSpeedChange;

    private float baseMovementSpeed;
    protected float movementSpeedPercent;
    public event Action<float> onMovementSpeedChange;

    private BigInteger baseSkillMultiplier;
    public event Action<BigInteger> onSkillDamageChange;

    public BigInteger BattleScore
    {
        get => battleScore;
        private set
        {
            battleScore = value;
            onBattleScoreChange?.Invoke(battleScore);
        }
    }

    public event Action<BigInteger> onBattleScoreChange;

    [Header("확인용")] public List<TempBuffStatus> buffList;
    private TempBuffStatus totalBuff;
    public List<PassiveStatus> passiveList;
    private BigInteger battleScore;

    public BigInteger InitBattleScore()
    {
        BattleScore =
            // 공격
            (baseAttack + attackPercent - totalBuff.attackBuff)
            // 체력
            + (baseMaxHealth + maxHealthPercent - totalBuff.healthBuff)
            // 뎀감
            + Mathf.FloorToInt((baseDamageReduction + damageReductionPercent - totalBuff.damageReductionBuff) * 10000)
            // 마나
            + (baseMaxMana + maxManaPercent - totalBuff.manaBuff)
            // 마나 회복
            + (baseManaRecovery + manaRecoveryPercent - totalBuff.manaRecoveryBuff)
            // 치확
            + Mathf.FloorToInt((baseCriticalChance + criticalChancePercent - totalBuff.critChanceBuff) * 10000)
            // 치증
            + (baseCriticalDamage + criticalDamagePercent - totalBuff.critDamageBuff)
            // 공속
            + Mathf.FloorToInt((baseAttackSpeed + attackSpeedPercent - totalBuff.attackSpeedBuff) * 10000)
            // 이동
            + Mathf.FloorToInt((baseMovementSpeed + movementSpeedPercent - totalBuff.movementSpeedBuff) * 10000)
            // 스증
            + (baseSkillMultiplier - totalBuff.skillDamageBuff);

        return BattleScore;
    }

    public BigInteger RecalculateStatus(BigInteger baseStat, BigInteger percent, ref BigInteger currentValue)
    {
        BigInteger percentIncrease = BigInteger.Multiply(baseStat, percent) / 100;
        currentValue = baseStat + percentIncrease;

        return currentValue;
    }

    public float RecalculateStatus(float baseStat, float percent, ref float currentValue)
    {
        float percentIncrease = baseStat * percent;
        currentValue = baseStat + percentIncrease;

        return currentValue;
    }

    public BigInteger AmplifyCalculateStatus(ref BigInteger percent, BigInteger changePercentValue,
        ref BigInteger currentValue, BigInteger baseStat)
    {
        percent += changePercentValue;
        return RecalculateStatus(baseStat, percent, ref currentValue);
    }

    public float AmplifyCalculateStatus(ref float percent, float changePercentValue, ref float currentValue,
        float baseStat)
    {
        percent += changePercentValue;
        return RecalculateStatus(baseStat, percent, ref currentValue);
    }

    public BigInteger AbsoluteCalculateStatus(ref BigInteger baseStat, BigInteger changeValue,
        ref BigInteger currentValue, BigInteger percent)
    {
        baseStat += changeValue;
        return RecalculateStatus(baseStat, percent, ref currentValue);
    }

    public float AbsoluteCalculateStatus(ref float baseStat, float changeValue, ref float currentValue, float percent)
    {
        baseStat += changeValue;
        return RecalculateStatus(baseStat, percent, ref currentValue);
    }

    // 레벨업 시 스텟 업그레이드
    public void LevelUpStatusUpgrade(int currentLv)
    {
        var score = new BigInteger(BattleScore.ToString());
        
        if (perLevelStatus.baseAttack > 0)
            ChangeBaseStat(EStatusType.ATK, (perLevelStatus.baseAttack));
        if (perLevelStatus.baseHealth > 0)
            ChangeBaseStat(EStatusType.HP, (perLevelStatus.baseHealth));
        if (perLevelStatus.baseDamageReduction > 0)
            ChangeBaseStat(EStatusType.DMG_REDU, perLevelStatus.baseDamageReduction);
        if (perLevelStatus.baseMana > 0)
            ChangeBaseStat(EStatusType.MP, (perLevelStatus.baseMana));
        if (perLevelStatus.baseManaRecovery > 0)
            ChangeBaseStat(EStatusType.MP_RECO, (perLevelStatus.baseManaRecovery));
        if (perLevelStatus.baseCritChance > 0)
            ChangeBaseStat(EStatusType.CRIT_CH, perLevelStatus.baseCritChance);
        if (perLevelStatus.baseCritDamage > 0)
            ChangeBaseStat(EStatusType.CRIT_DMG, (perLevelStatus.baseCritDamage));
        if (perLevelStatus.baseAttackSpeed > 0)
            ChangeBaseStat(EStatusType.ATK_SPD, perLevelStatus.baseAttackSpeed);
        if (perLevelStatus.baseMovementSpeed > 0)
            ChangeBaseStat(EStatusType.MOV_SPD, perLevelStatus.baseMovementSpeed);

        InitBattleScore();
        MessageUIManager.instance.ShowPower(BattleScore, BattleScore - score);
    }

    public void SetStatusAtLevel(int atLevel)
    {
        ChangeBaseStat(EStatusType.ATK, (perLevelStatus.baseAttack * (atLevel - 1)));
        ChangeBaseStat(EStatusType.HP, (perLevelStatus.baseHealth * (atLevel - 1)));
        ChangeBaseStat(EStatusType.DMG_REDU, perLevelStatus.baseDamageReduction * (atLevel - 1));
        ChangeBaseStat(EStatusType.MP, (perLevelStatus.baseMana * (atLevel - 1)));
        ChangeBaseStat(EStatusType.MP_RECO, (perLevelStatus.baseManaRecovery * (atLevel - 1)));
        ChangeBaseStat(EStatusType.CRIT_CH, perLevelStatus.baseCritChance * (atLevel - 1));
        ChangeBaseStat(EStatusType.CRIT_DMG, (perLevelStatus.baseCritDamage * (atLevel - 1)));
        ChangeBaseStat(EStatusType.ATK_SPD, perLevelStatus.baseAttackSpeed * (atLevel - 1));
        ChangeBaseStat(EStatusType.MOV_SPD, perLevelStatus.baseMovementSpeed * (atLevel - 1));
    }

    public void InitStatus()
    {
        baseAttack = level1Status.baseAttack;
        baseMaxHealth = level1Status.baseHealth;
        baseDamageReduction = level1Status.baseDamageReduction;
        baseMaxMana = level1Status.baseMana;
        baseManaRecovery = level1Status.baseManaRecovery;
        baseCriticalChance = level1Status.baseCritChance;
        baseCriticalDamage = level1Status.baseCritDamage;
        baseAttackSpeed = level1Status.baseAttackSpeed;
        baseMovementSpeed = level1Status.baseMovementSpeed;
        baseSkillMultiplier = 0;

        attackPercent = 0;
        maxHealthPercent = 0;
        damageReductionPercent = 0;
        maxManaPercent = 0;
        manaRecoveryPercent = 0;
        criticalChancePercent = 0;
        criticalDamagePercent = 0;
        attackSpeedPercent = 0;
        movementSpeedPercent = 0;

#if UNITY_EDITOR
        onAttackChange += x => testAttack = x.ToString();
        onMaxHealthChange += x => testHealth = x.ToString();
        onMaxManaChange += x => testMana = x.ToString();
        onManaRecoveryChange += x => testManaRecovery = x.ToString();
        onCriticalDamageChange += x => testCriticalDamage = x.ToString();
        onSkillDamageChange += x => testSkillDamage = x.ToString();
#endif
        InitBattleScore();
    }

    public void ChangeBaseStat(EStatusType type, int value)
    {
        switch (type)
        {
            case EStatusType.ATK:
                AbsoluteCalculateStatus(ref baseAttack, value, ref currentAttack, attackPercent);
                onAttackChange?.Invoke(currentAttack);
                break;
            case EStatusType.HP:
                AbsoluteCalculateStatus(ref baseMaxHealth, value, ref currentMaxHealth, maxHealthPercent);
                onMaxHealthChange?.Invoke(currentMaxHealth);
                break;
            case EStatusType.MP:
                AbsoluteCalculateStatus(ref baseMaxMana, value, ref currentMaxMana, maxManaPercent);
                onMaxManaChange?.Invoke(currentMaxMana);
                break;
            case EStatusType.MP_RECO:
                AbsoluteCalculateStatus(ref baseManaRecovery, value, ref currentManaRecovery, manaRecoveryPercent);
                onManaRecoveryChange?.Invoke(currentManaRecovery);
                break;
            case EStatusType.CRIT_DMG:
                AbsoluteCalculateStatus(ref baseCriticalDamage, value, ref currentCriticalDamage,
                    criticalDamagePercent);
                onCriticalDamageChange?.Invoke(currentCriticalDamage);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void ChangeBaseStat(EStatusType type, BigInteger value)
    {
        switch (type)
        {
            case EStatusType.ATK:
                AbsoluteCalculateStatus(ref baseAttack, value, ref currentAttack, attackPercent);
                onAttackChange?.Invoke(currentAttack);
                break;
            case EStatusType.HP:
                AbsoluteCalculateStatus(ref baseMaxHealth, value, ref currentMaxHealth, maxHealthPercent);
                onMaxHealthChange?.Invoke(currentMaxHealth);
                break;
            case EStatusType.MP:
                AbsoluteCalculateStatus(ref baseMaxMana, value, ref currentMaxMana, maxManaPercent);
                onMaxManaChange?.Invoke(currentMaxMana);
                break;
            case EStatusType.MP_RECO:
                AbsoluteCalculateStatus(ref baseManaRecovery, value, ref currentManaRecovery, manaRecoveryPercent);
                onManaRecoveryChange?.Invoke(currentManaRecovery);
                break;
            case EStatusType.CRIT_DMG:
                AbsoluteCalculateStatus(ref baseCriticalDamage, value, ref currentCriticalDamage,
                    criticalDamagePercent);
                onCriticalDamageChange?.Invoke(currentCriticalDamage);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void ChangeBaseStat(EStatusType type, float value)
    {
        switch (type)
        {
            case EStatusType.DMG_REDU:
                AbsoluteCalculateStatus(ref baseDamageReduction, value, ref currentDamageReduction,
                    damageReductionPercent);
                onDamageReductionChange?.Invoke(currentDamageReduction);
                break;
            case EStatusType.CRIT_CH:
                AbsoluteCalculateStatus(ref baseCriticalChance, value, ref currentCriticalChance,
                    criticalChancePercent);
                onCriticalChanceChange?.Invoke(currentCriticalChance);
                break;
            case EStatusType.ATK_SPD:
                AbsoluteCalculateStatus(ref baseAttackSpeed, value, ref currentAttackSpeed, attackSpeedPercent);
                onAttackSpeedChange?.Invoke(currentAttackSpeed);
                break;
            case EStatusType.MOV_SPD:
                AbsoluteCalculateStatus(ref baseMovementSpeed, value, ref currentMovementSpeed, movementSpeedPercent);
                onMovementSpeedChange?.Invoke(currentMovementSpeed);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void ChangePercentStat(EStatusType type, BigInteger percent)
    {
        switch (type)
        {
            case EStatusType.ATK:
                AmplifyCalculateStatus(ref attackPercent, percent, ref currentAttack, baseAttack);
                onAttackChange?.Invoke(currentAttack);
                break;
            case EStatusType.HP:
                AmplifyCalculateStatus(ref maxHealthPercent, percent, ref currentMaxHealth, baseMaxHealth);
                onMaxHealthChange?.Invoke(currentMaxHealth);
                break;
            case EStatusType.MP:
                AmplifyCalculateStatus(ref maxManaPercent, percent, ref currentMaxMana, baseMaxMana);
                onMaxManaChange?.Invoke(currentMaxMana);
                break;
            case EStatusType.MP_RECO:
                AmplifyCalculateStatus(ref manaRecoveryPercent, percent, ref currentManaRecovery, baseManaRecovery);
                onManaRecoveryChange?.Invoke(currentManaRecovery);
                break;
            case EStatusType.CRIT_DMG:
                AmplifyCalculateStatus(ref criticalDamagePercent, percent, ref currentCriticalDamage,
                    baseCriticalDamage);
                onCriticalDamageChange?.Invoke(currentCriticalDamage);
                break;
            case EStatusType.SKILL_DMG:
                AbsoluteCalculateStatus(ref baseSkillMultiplier, percent, ref currentSkillDamage, 100);
                onSkillDamageChange?.Invoke(baseSkillMultiplier);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void ChangePercentStat(EStatusType type, float percent)
    {
        switch (type)
        {
            case EStatusType.DMG_REDU:
                AmplifyCalculateStatus(ref damageReductionPercent, percent, ref currentDamageReduction,
                    baseDamageReduction);
                onDamageReductionChange?.Invoke(currentDamageReduction);
                break;
            case EStatusType.CRIT_CH:
                AmplifyCalculateStatus(ref criticalChancePercent, percent, ref currentCriticalChance,
                    baseCriticalChance);
                onCriticalChanceChange?.Invoke(currentCriticalChance);
                break;
            case EStatusType.ATK_SPD:
                AmplifyCalculateStatus(ref attackSpeedPercent, percent, ref currentAttackSpeed, baseAttackSpeed);
                onAttackSpeedChange?.Invoke(currentAttackSpeed);
                break;
            case EStatusType.MOV_SPD:
                AmplifyCalculateStatus(ref movementSpeedPercent, percent, ref currentMovementSpeed, baseMovementSpeed);
                onMovementSpeedChange?.Invoke(currentMovementSpeed);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void IncreaseBaseStat(EStatusType initType, BigInteger increase)
    {
        ChangeBaseStat(initType, increase);
    }

    public void IncreaseBaseStat(EStatusType initType, int increase)
    {
        ChangeBaseStat(initType, increase);
    }

    public void IncreaseBaseStat(EStatusType initType, float increase)
    {
        ChangeBaseStat(initType, increase);
    }

    public void AddBuffToList(TempBuffStatus tempBuff)
    {
        buffList.Add(tempBuff);
        if (tempBuff.attackBuff > 0)
            ChangePercentStat(EStatusType.ATK, new BigInteger(tempBuff.attackBuff));
        if (tempBuff.healthBuff > 0)
            ChangePercentStat(EStatusType.HP, new BigInteger(tempBuff.healthBuff));
        if (tempBuff.damageReductionBuff > 0)
            ChangePercentStat(EStatusType.DMG_REDU, tempBuff.damageReductionBuff);
        if (tempBuff.manaBuff > 0)
            ChangePercentStat(EStatusType.MP, new BigInteger(tempBuff.manaBuff));
        if (tempBuff.manaRecoveryBuff > 0)
            ChangePercentStat(EStatusType.MP_RECO, new BigInteger(tempBuff.manaRecoveryBuff));
        if (tempBuff.critChanceBuff > 0)
            ChangePercentStat(EStatusType.CRIT_CH, tempBuff.critChanceBuff);
        if (tempBuff.critDamageBuff > 0)
            ChangePercentStat(EStatusType.CRIT_DMG, new BigInteger(tempBuff.critDamageBuff));
        if (tempBuff.attackSpeedBuff > 0)
            ChangePercentStat(EStatusType.ATK_SPD, tempBuff.attackSpeedBuff);
        if (tempBuff.movementSpeedBuff > 0)
            ChangePercentStat(EStatusType.MOV_SPD, tempBuff.movementSpeedBuff);
        if (tempBuff.skillDamageBuff > 0)
            ChangePercentStat(EStatusType.SKILL_DMG, new BigInteger(tempBuff.skillDamageBuff));

        totalBuff += tempBuff;
    }

    public void RemoveBuffFromList(TempBuffStatus tempBuff)
    {
        buffList.Remove(tempBuff);
        if (tempBuff.attackBuff > 0)
            ChangePercentStat(EStatusType.ATK, new BigInteger(-tempBuff.attackBuff));
        if (tempBuff.healthBuff > 0)
            ChangePercentStat(EStatusType.HP, new BigInteger(-tempBuff.healthBuff));
        if (tempBuff.damageReductionBuff > 0)
            ChangePercentStat(EStatusType.DMG_REDU, -tempBuff.damageReductionBuff);
        if (tempBuff.manaBuff > 0)
            ChangePercentStat(EStatusType.MP, new BigInteger(-tempBuff.manaBuff));
        if (tempBuff.manaRecoveryBuff > 0)
            ChangePercentStat(EStatusType.MP_RECO, new BigInteger(-tempBuff.manaRecoveryBuff));
        if (tempBuff.critChanceBuff > 0)
            ChangePercentStat(EStatusType.CRIT_CH, -tempBuff.critChanceBuff);
        if (tempBuff.critDamageBuff > 0)
            ChangePercentStat(EStatusType.CRIT_DMG, new BigInteger(-tempBuff.critDamageBuff));
        if (tempBuff.attackSpeedBuff > 0)
            ChangePercentStat(EStatusType.ATK_SPD, -tempBuff.attackSpeedBuff);
        if (tempBuff.movementSpeedBuff > 0)
            ChangePercentStat(EStatusType.MOV_SPD, -tempBuff.movementSpeedBuff);
        if (tempBuff.skillDamageBuff > 0)
            ChangePercentStat(EStatusType.SKILL_DMG, new BigInteger(-tempBuff.skillDamageBuff));

        totalBuff -= tempBuff;
    }

    public void ApplyPassiveStatus(PassiveStatus passive)
    {
        passiveList.Add(passive);
        switch (passive.target)
        {
            case EStatusType.ATK:
            case EStatusType.HP:
            case EStatusType.MP:
            case EStatusType.MP_RECO:
            case EStatusType.CRIT_DMG:
            case EStatusType.SKILL_DMG:
                ChangePercentStat(passive.target, new BigInteger(passive.buff));
                break;
            case EStatusType.DMG_REDU:
            case EStatusType.CRIT_CH:
            case EStatusType.ATK_SPD:
            case EStatusType.MOV_SPD:
                ChangePercentStat(passive.target, passive.buff / 100f);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void RemovePassiveStatus(PassiveStatus passive)
    {
        passiveList.Remove(passive);
        switch (passive.target)
        {
            case EStatusType.ATK:
            case EStatusType.HP:
            case EStatusType.MP:
            case EStatusType.MP_RECO:
            case EStatusType.CRIT_DMG:
            case EStatusType.SKILL_DMG:
                ChangePercentStat(passive.target, new BigInteger(-passive.buff));
                break;
            case EStatusType.DMG_REDU:
            case EStatusType.CRIT_CH:
            case EStatusType.ATK_SPD:
            case EStatusType.MOV_SPD:
                ChangePercentStat(passive.target, -(float)passive.buff / 100);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}