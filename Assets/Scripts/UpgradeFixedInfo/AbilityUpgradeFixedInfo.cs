using System;
using Defines;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "SO/AbilityUpgradeFixedInfo", fileName = "AbilityUpgradeFixedInfo")]
public class AbilityUpgradeFixedInfo : ScriptableObject
{
    public string title;

    public EStatusType statusType;

    public ECurrencyType currencyType;
    public int baseCost;
}