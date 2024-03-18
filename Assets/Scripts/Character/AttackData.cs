using System;
using Character.Monster;
using Keiwando.BigInteger;
using Random = UnityEngine.Random;

[Serializable]
public class AttackData
{
    private BigInteger multiplier;
    private CurrentStatus status;
    public int knockback;
    public bool isContinuous;
    public int attackCount;
    public float tickUnitTime;

    public AttackData(PlayerData data, int knockback = 0, BigInteger multiplier = null, int attackCount = 1, bool isContinuous = false, float tickUnitTime = 0.1f)
    {
        status = data.status;
        this.knockback = knockback;

        if (!ReferenceEquals(multiplier, null))
        {
            this.multiplier = multiplier;
        }

        this.attackCount = attackCount;
        this.isContinuous = isContinuous;
        this.tickUnitTime = tickUnitTime;
    }

    public AttackData(MonsterData data, int knockback = 0, BigInteger multiplier = null, int attackCount = 1, bool isContinuous = false, float tickUnitTime = 0.1f)
    {
        status = data.status;
        this.knockback = knockback;
        
        if (!ReferenceEquals(multiplier, null))
        {
            this.multiplier = multiplier;
        }
        
        this.attackCount = attackCount;
        this.isContinuous = isContinuous;
        this.tickUnitTime = tickUnitTime;
    }

    public virtual BigInteger GetDamage(out bool isCrit)
    {
        var rand = Random.Range(90, 100);
        if (ReferenceEquals(multiplier, null))
        {
            if (Random.Range(0f, 1f) < status.currentCriticalChance)
            {
                isCrit = true;
                return status.currentAttack * status.currentCriticalDamage * rand / 10000;
            }

            isCrit = false;
            return status.currentAttack * rand / 100;
        }
        else
        {
            if (Random.Range(0f, 1f) < status.currentCriticalChance)
            {
                isCrit = true;
                return status.currentAttack * status.currentCriticalDamage * multiplier * rand / 1000000;
            }

            isCrit = false;
            return status.currentAttack *  multiplier * rand / 10000;
        }
    }

    public virtual int GetKnockBack()
    {
        return knockback;
    }

    public virtual bool IsKnockBack()
    {
        return knockback > 0;
    }
}