using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using Keiwando.BigInteger;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "SO/DungeonData", fileName = "DungeonData")]
public class DungeonData : ScriptableObject
{
    public ENormalRewardType rewardType;
    public ECurrencyType invitationType;
    public string dungeonSubName;
    public string goalInstruction;
    [field: SerializeField] public EDungeonType dungeonType { get; protected set; }
    [field: SerializeField] public StageDataSO stageSO { get; protected set; }
    [field: SerializeField][field: Range(1, 100)] public int dungeonLevel { get; protected set; } = 1;
    [field: SerializeField] public float goalTime { get; protected set; } = 60;
    [field: SerializeField] public int goalKillCount { get; protected set; } = 100;
    [field: SerializeField] public int baseEarnPerOne { get; protected set; }
    [field: SerializeField] public BigInteger earnPerOne { get; protected set; }
    [field: SerializeField] public int increaceEarn { get; protected set; }
    [field: SerializeField] public Sprite dungeonImage { get; protected set; }
    [field: SerializeField] public string description { get; protected set; }
    public event Action<int> onDungeonLevelUP;
    public void LevelUpEarnPerOne()
    {
        earnPerOne += earnPerOne * increaceEarn / 100;
    }

    public void InitReward()
    {
        earnPerOne = (baseEarnPerOne * BigInteger.Pow(100+increaceEarn, dungeonLevel)) / BigInteger.Pow(100, dungeonLevel);
    }

    public BigInteger GetTotalReward()
    {
        if (dungeonLevel == 1)
            earnPerOne = baseEarnPerOne;
        return earnPerOne * goalKillCount;
    }

    public BigInteger GetEnemyAttack()
    {
        BigInteger attack;
        switch (dungeonType)
        {
            case EDungeonType.Gold:
            // case EDungeonType.Enhance:
                attack = stageSO.BasicMonstersBaseStatus[0].baseAttack +
                         (BigInteger)stageSO.BasicMonstersPerLevel[0].baseAttack * dungeonLevel;
                break;
            case EDungeonType.Awaken:
            case EDungeonType.Enhance:
                attack = stageSO.BossMonsterBaseStatus.baseAttack +
                         (BigInteger)stageSO.BossMonsterPerLevel.baseAttack * dungeonLevel;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return attack;
    }

    public BigInteger GetEnemyHealth()
    {
        BigInteger health;
        switch (dungeonType)
        {
            case EDungeonType.Gold:
            // case EDungeonType.Enhance:
                health = stageSO.BasicMonstersBaseStatus[0].baseHealth +
                         (BigInteger)stageSO.BasicMonstersPerLevel[0].baseHealth * dungeonLevel;
                break;
            case EDungeonType.Awaken:
            case EDungeonType.Enhance:
                health = stageSO.BossMonsterBaseStatus.baseHealth +
                         (BigInteger)stageSO.BossMonsterPerLevel.baseHealth * dungeonLevel;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return health;
    }
    
    // Save & Load
    public void LevelUp()
    {
        LevelUpEarnPerOne();
        ++dungeonLevel;
        Save();
        onDungeonLevelUP?.Invoke(dungeonLevel);
    }

    public void Save()
    {
        DataManager.Instance.Save($"{dungeonType.ToString()}_{nameof(dungeonLevel)}", dungeonLevel);
        DataManager.Instance.Save($"{dungeonType.ToString()}_{nameof(earnPerOne)}", earnPerOne.ToString());
    }

    public void Load()
    {
        dungeonLevel = DataManager.Instance.Load($"{dungeonType.ToString()}_{nameof(dungeonLevel)}", 1);
        earnPerOne = new BigInteger(DataManager.Instance.Load<string>($"{dungeonType.ToString()}_{nameof(earnPerOne)}", baseEarnPerOne.ToString()));
    }
}
