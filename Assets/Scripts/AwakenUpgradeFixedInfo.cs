using System;
using Defines;
using Keiwando.BigInteger;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "SO/AwakenUpgradeFixedInfo", fileName = "AwakenUpgradeFixedInfo")]
public class AwakenUpgradeFixedInfo : ScriptableObject
{
    public string gemName;
    public string title;
    public int maxLevel;

    // 업글 관련
    public EStatusType statusType;

    [Header("ATK, HP, MP, MP_RECO, CRIT_DMG")]
    public int upgradePerLevelInt;

    [Header("DMG_REDU, CRIT_CH, ATK_SPD, MOV_SPD")]
    public float upgradePerLevelFloat;

    // 비용 관련
    public ECurrencyType currencyType;
    public int baseCost;
    public int increaseCostPerLevel;

    // 꾸미기 관련
    public Sprite image;
}