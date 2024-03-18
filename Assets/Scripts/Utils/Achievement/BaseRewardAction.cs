using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Defines;
using Keiwando.BigInteger;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

[Serializable]
public class BaseRewardAction
{
    public EQuestRewardType type;
    protected Func<int, bool> rewardAction;
    protected Func<BigInteger, bool> rewardBigIntegerAction;

    public virtual void InitializeReward(EQuestRewardType initType)
    {
        type = initType;
        switch (initType)
        {
            case EQuestRewardType.Gold:
            case EQuestRewardType.Dia:
            case EQuestRewardType.EnhanceStone:
            case EQuestRewardType.AwakenStone:
            case EQuestRewardType.WeaponSummonTicket:
            case EQuestRewardType.ArmorSummonTicket:
            case EQuestRewardType.GoldInvitation:
            case EQuestRewardType.AwakenInvitation:
            case EQuestRewardType.EnhanceInvitation:
                rewardBigIntegerAction = (x) =>
                {
                    CurrencyManager.instance.AddCurrency((ECurrencyType)initType, x);
                    return true;
                };
                rewardAction = (x) =>
                {
                    CurrencyManager.instance.AddCurrency((ECurrencyType)initType, x);
                    return true;
                };
                break;
            case EQuestRewardType.BaseAtk:
            case EQuestRewardType.BaseHp:
            case EQuestRewardType.BaseDef:
                rewardBigIntegerAction = (x) =>
                {
                    PlayerManager.instance.status.IncreaseBaseStat((EStatusType)(initType - EQuestRewardType.BaseAtk), x);
                    return true;
                };
                rewardAction = (x) =>
                {
                    PlayerManager.instance.status.IncreaseBaseStat((EStatusType)(initType - EQuestRewardType.BaseAtk), x);
                    return true;
                };
                break;
            case EQuestRewardType.BaseCritCh:
            case EQuestRewardType.BaseCritDmg:
            case EQuestRewardType.BaseAtkSpd:
                rewardBigIntegerAction = (x) =>
                {
                    PlayerManager.instance.status.IncreaseBaseStat((EStatusType)(initType - EQuestRewardType.BaseAtk),
                        x / 100);
                    return true;
                };
                rewardAction = (x) =>
                {
                    PlayerManager.instance.status.IncreaseBaseStat((EStatusType)(initType - EQuestRewardType.BaseAtk),
                        x / 100);
                    return true;
                };
                break;
            case EQuestRewardType.Exp:
                rewardBigIntegerAction = (x) =>
                {
                    PlayerManager.instance.levelSystem.EarnExp(x);
                    return true;
                };
                rewardAction = x =>
                {
                    PlayerManager.instance.levelSystem.EarnExp(x);
                    return true;
                };
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public virtual bool GetReward(int amount)
    {
        Debug.Assert(amount < int.MaxValue);
        return rewardAction(amount);
    }

    public virtual bool GetReward(BigInteger amount)
    {
        return rewardBigIntegerAction(amount);
    }
}