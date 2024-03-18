using System;
using System.Collections;
using System.Collections.Generic;
using Character.Monster;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "SO/StageData")]
public class StageDataSO : ScriptableObject
{
    [Header("Stage setting")]
    [SerializeField] string stageName;
    int stageSection = 0;
    int stageNumber = 0;

    [SerializeField] int goalKillCount;
    [SerializeField] float bossTimeLimit;
    [SerializeField] int maxSpawn;
    [SerializeField] private GameObject mapPrefab;

    [Header("Boss Monster")]
    [SerializeField] string bossName;
    [SerializeField] string bossDescription;
    [SerializeField] BaseStatus bossMonsterBaseStatus;
    [SerializeField] BaseStatus bossMonsterPerLevel;
    [SerializeField] MonsterData bossMonsterPrefab;
    [SerializeField] MonsterDropData[] bossMonsterReward;
    [SerializeField] private Vector3 bossSpawnPosition;
    [SerializeField] private BaseStatus number20BossBonusStatus;

    [Header("Normal/Elite Monster")]
    [SerializeField] BaseStatus[] basicMonstersBaseStatus;
    [SerializeField] BaseStatus[] basicMonstersPerLevel;
    [SerializeField] MonsterData[] basicMonstersPrefab;
    [SerializeField] MonsterDropData[] basicMonstersReward;
    [SerializeField] private Vector3[] basicMonsterSpawnPosition;

    public string StageName => stageName;
    public int StageSection { get => stageSection; set => stageSection = value; }
    public int StageNumber { get => stageNumber; set => stageNumber = value; }

    public string BossName => bossName;
    public string BossDescription => bossDescription;

    public int GoalKillCount => goalKillCount;
    public float BossTimeLimit => bossTimeLimit;
    public int MaxSpawn => maxSpawn;
    public GameObject Map => mapPrefab;

    public BaseStatus BossMonsterBaseStatus => bossMonsterBaseStatus;
    public BaseStatus BossMonsterPerLevel => bossMonsterPerLevel;
    public MonsterData BossMonsterPrefab => bossMonsterPrefab;
    public Vector3 BossSpawnPosition => bossSpawnPosition;
    public BaseStatus Number20BossBonusStatus => number20BossBonusStatus;
    
    public BaseStatus[] BasicMonstersBaseStatus => basicMonstersBaseStatus;
    public BaseStatus[] BasicMonstersPerLevel => basicMonstersPerLevel;
    public MonsterData[] BasicMonstersPrefab => basicMonstersPrefab;
    public MonsterDropData[] BossMonsterReward => bossMonsterReward;
    public MonsterDropData[] BasicMonstersReward => basicMonstersReward;
    public Vector3[] BasicMonsterSpawnPosition => basicMonsterSpawnPosition;

    //rewards (gold, expPerMonster, etc.)
}