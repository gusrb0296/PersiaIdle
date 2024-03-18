using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Character.Monster;
using Defines;
using Keiwando.BigInteger;
using UnityEditor;
using UnityEngine;
using Utils;

public class StageManager : MonoBehaviour
{
    public static StageManager instance;
    public float clearPanelTime = 2f;

    private PlayerData player => PlayerManager.instance.player;
    
    private EStageState state;

    private MonsterSpawner monsterSpawner;
    [SerializeField] private StageDataSO[] stageDatas;
    public DungeonData goldDungeon;
    public DungeonData awakenDungeon;
    public DungeonData enhanceDungeon;
    private DungeonData currentDungeon;

    private int maxClearStage;
    private int maxClearSection;
    private int maxClearNumber;
    public int MaxClearStage => maxClearStage;
    public int MaxClearSection => maxClearSection;
    public int MaxClearNumber => maxClearNumber;

    private StageDataSO currentStageData;
    public int currentStage { get; protected set; }
    private int currentSection;
    private int currentNumber;
    private int currentKillCount;
    private int goalKillCount;

    private MonsterData currentBoss;
    private GameObject currentMap;

    private bool isBossStageAvailable;

    private bool isOnTimer;

    private Coroutine interModeCoroutine;
    private Coroutine timerCoroutine;

    public event Action<Defines.EStageState> OnStateChange;
    public event Action<string> OnStageChange;
    public event Action<float, float> OnTimerChanged;
    public event Action<int, int> OnMonsterKill;
    public event Action<BigInteger, BigInteger> OnBossHPChange;
    public event Action<bool> OnBossStageValidityChange;
    public event Action<int> onMaxClearStageChanged;

    private bool autoBossChallenge;

    public bool AutoBossChallenge
    {
        get { return autoBossChallenge; }
        set { autoBossChallenge = value; }
    }

    private void Awake()
    {
        instance = this;
    }

    public void InitStageManager()
    {
        // InitCollections();
        InitMonsterSpawner();
        LoadDatas();
        UpdateCurrentStageData();

        monsterSpawner.StartInfiniteGenerating();

        string stageName =
            $"{currentStageData.StageName} {(stageDatas.Length * (currentSection - 1) + currentStage + 1).ToString()}-{currentStageData.StageNumber.ToString()}";
        var ui = UIManager.instance.TryGetUI<UIStageBar>();
        ui.ShowNormalStageUI(stageName);

        // var ui = UIManager.instance.TryGetUI<UIStageBar>();
        // OnBossHPChange += ui.UpdateHealth;
        // OnTimerChanged += ui.UpdateTimer;
        OnMonsterKill += player.health.HealPerKill;
        player.controller.onDeathStart += UIManager.instance.TryGetUI<UIStageFail>().ShowUI;
    }

    // private void InitCollections()
    // {
    //     difficultyDic = new Dictionary<Defines.EStageDifficulty, string>();
    //
    //     difficultyDic[Defines.EStageDifficulty.Easy] = Strings.StageDifficulties.EASY;
    //     difficultyDic[Defines.EStageDifficulty.Normal] = Strings.StageDifficulties.NORMAL;
    //     difficultyDic[Defines.EStageDifficulty.Hard] = Strings.StageDifficulties.HARD;
    // }

    private void InitMonsterSpawner()
    {
        MonsterSpawner obj = Resources.Load<MonsterSpawner>("Prefabs/MonsterSpawner");
        monsterSpawner = Instantiate(obj);
    }

    private void LoadDatas()
    {
        // stageDatas = Resources.LoadAll<StageDataSO>("ScriptableObjects/StageDataSO");

        //Temp logic
        // currentDifficulty = Defines.EStageDifficulty.Easy;
        Load();
        currentNumber = maxClearNumber;
        currentSection = maxClearSection;
        currentStage = maxClearStage;

        foreach (var stage in stageDatas)
        {
            foreach (var reward in stage.BasicMonstersReward)
            {
                reward.InitCurrentReward();
            }

            foreach (var reward in stage.BossMonsterReward)
            {
                reward.InitCurrentReward();
            }
        }

        awakenDungeon.Load();
        goldDungeon.Load();
        enhanceDungeon.Load();
        // goldDungeon.InitReward();
        // awakenDungeon.InitReward();
    }

    private void UpdateCurrentStageData()
    {
        state = Defines.EStageState.Normal;
        OnStateChange?.Invoke(state);
        ClearSubscribe();
        monsterSpawner.ClearMonsters();

        currentStageData = stageDatas[currentStage];
        currentStageData.StageSection = currentSection;
        currentStageData.StageNumber = currentNumber;
        goalKillCount = currentStageData.GoalKillCount;

        Destroy(currentMap);
        currentMap = Instantiate(stageDatas[currentStage].Map);

        monsterSpawner.SetStageData(currentStageData);

        // monsterSpawner.StartInfiniteGenerating();

        string stageName =
            $"{currentStageData.StageName} {(stageDatas.Length * (currentSection - 1) + currentStage + 1).ToString()}-{currentStageData.StageNumber.ToString()}";

        // var ui = UIManager.instance.TryGetUI<UIStageBar>();
        // ui.ShowNormalStageUI(stageName);

        OnStageChange?.Invoke(stageName);
        player.controller.onDeathStart += InitStageForResurrection;
    }

    private void SetDungeonData(DungeonData data)
    {
        state = Defines.EStageState.Dungeon;
        OnStateChange?.Invoke(state);
        ClearSubscribe();
        monsterSpawner.ClearMonsters();

        string stageName = $"{data.dungeonSubName} {CustomText.SetColor($"Lv.{data.dungeonLevel}", EColorType.Green)}";
        switch (data.dungeonType)
        {
            case EDungeonType.Gold:
            {
                // currentStageData = data.stageSO;
                currentKillCount = 0;
                goalKillCount = data.goalKillCount;

                monsterSpawner.SetDungeonData(data);

                Destroy(currentMap);
                currentMap = Instantiate(data.stageSO.Map);

                monsterSpawner.StartGoldDungeonGenerate();

                var ui = UIManager.instance.TryGetUI<UIStageBar>();
                ui.ShowProgressUI(EStageState.Dungeon, stageName, data.stageSO.StageName, EIconType.Skull,
                    EIconType.Clock);
                OnMonsterKill += ui.UpdateHealth;
                OnTimerChanged += ui.UpdateTimer;
                // var init = Mathf.FloorToInt(data.goalTime);
                // ui.InitDownSlider(init, init);
                ui.InitUpSlider(data.goalKillCount, 0);
                ui.InitDownSlider();

                OnMonsterKill += SwitchToDungeonReward;
                break;
            }
            case EDungeonType.Awaken:
            {
                monsterSpawner.SetDungeonData(data);
                //
                // monsterSpawner.StartAwakenDungeonGenerate();
                Destroy(currentMap);
                currentMap = Instantiate(data.stageSO.Map);

                currentBoss = monsterSpawner.InstantiateAwakenBoss(data);
                currentBoss.controller.onDeathEnd += () => SwitchToReward(data);

                var ui = UIManager.instance.TryGetUI<UIStageBar>();
                ui.ShowProgressUI(EStageState.Dungeon, stageName, data.stageSO.StageName, EIconType.Skull,
                    EIconType.Clock);

                // var init = Mathf.FloorToInt(data.goalTime);
                // ui.InitDownSlider(init, init);
                ui.InitUpSlider(100, 100);
                ui.InitDownSlider();

                OnBossHPChange += ui.UpdateHealth;
                OnTimerChanged += ui.UpdateTimer;

                ManageBossCallback(true);

                // currentBoss.Idle();
                break;
            }
            case EDungeonType.Enhance:
            {
                monsterSpawner.SetDungeonData(data);

                Destroy(currentMap);
                currentMap = Instantiate(data.stageSO.Map);
                
                // monsterSpawner.StartGoldDungeonGenerate();
                currentBoss = monsterSpawner.InstantiateAwakenBoss(data);
                CameraController.SetFollow(currentBoss.gameObject);
                currentBoss.controller.onDeathEnd += () => SwitchToReward(data);
                
                var ui = UIManager.instance.TryGetUI<UIStageBar>();
                // ui.ShowProgressUI(EStageState.Dungeon, stageName, data.dungeonSubName, EIconType.Skull, EIconType.Clock);
                ui.ShowProgressUI(EStageState.Dungeon, stageName, data.stageSO.StageName, EIconType.Skull, EIconType.Clock);
                
                // OnMonsterKill += ui.UpdateHealth;
                // OnTimerChanged += ui.UpdateTimer;
                
                // ui.InitUpSlider(data.goalKillCount, 0);
                ui.InitUpSlider(100, 100);
                ui.InitDownSlider();
                
                OnBossHPChange += ui.UpdateHealth;
                OnTimerChanged += ui.UpdateTimer;
                
                ManageBossCallback(true);

                // OnMonsterKill += SwitchToDungeonReward;
                // currentBoss.Idle();
                currentBoss.controller.onDeathStart += () => CameraController.SetFollow(player.gameObject);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        isOnTimer = true;
        OnStageChange?.Invoke(stageName);
        // subscribe onMonsterKill that change to reward screen
        currentDungeon = data;
        player.controller.onDeathStart += InitDungeonForResurrection;
    }

    public void AddKillCount()
    {
        currentKillCount++;

        if (currentKillCount > goalKillCount)
        {
            currentKillCount = goalKillCount;
            // return;
        }

        OnMonsterKill?.Invoke(currentKillCount, goalKillCount);
    }

    // public void SwitchToBossStage(int currentKill, int goalKill)
    // {
    //     if (currentKill == goalKill)
    //     {
    //         SwitchToBossStage();
    //     }
    // }

    public void SwitchToNormalStage(int currentKill, int goalKill)
    {
        if (currentKill == goalKill) InitStage();
    }

    public void SwitchToDungeonReward(int currentKill, int goalKill)
    {
        if (currentKill == goalKill) SwitchToReward(currentDungeon);
    }

    private void InitStage()
    {
        monsterSpawner.StopGenerating();
        monsterSpawner.ClearMonsters();

        if (interModeCoroutine != null) StopCoroutine(interModeCoroutine);
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);

        interModeCoroutine = StartCoroutine(InitNormalState());
    }

    private void InitStageForResurrection()
    {
        AutoBossChallenge = false;
        UIManager.instance.TryGetUI<UIStageBar>().SetAutoBossWithoutNotify(false);

        monsterSpawner.StopGenerating();
        monsterSpawner.ClearMonsters();

        if (interModeCoroutine != null) StopCoroutine(interModeCoroutine);
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);

        interModeCoroutine = StartCoroutine(InitPlayerResurrectionState());
    }

    public void SwitchToBossStage()
    {
        if (state != EStageState.Normal) return;

        monsterSpawner.StopGenerating();
        monsterSpawner.ClearMonsters();

        if (interModeCoroutine != null) StopCoroutine(interModeCoroutine);
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);

        isOnTimer = true;
        interModeCoroutine = StartCoroutine(InitBossStage());
        timerCoroutine = StartCoroutine(Timer(currentStageData.BossTimeLimit, BossModeFailed));
    }

    private void BossModeSuccess(MonsterData monster)
    {
        ManageBossCallback(false);
        SetBossModeValidity(false);

        isOnTimer = false; //TODO: Delete

        currentKillCount = 0;

        ++currentNumber;
        if (currentNumber > 20)
        {
            ++currentStage;
            if (currentStage >= stageDatas.Length)
            {
                currentStage = 0;
                ++currentSection;
            }

            currentNumber = 1;
        }

        UpdateMaxStage(currentStage, currentSection, currentNumber);

        // TODO 전투 결과창 보여주고, 보상 지급하기
        UIManager.instance.TryGetUI<UIStageClearPanel>().ShowUI(currentStageData.BossMonsterReward);
        foreach (var rewardData in currentStageData.BossMonsterReward)
            GameManager.instance.GetReward(rewardData.rewardType, rewardData.straightRewardAmount);

        StopCoroutine(timerCoroutine);
        if (interModeCoroutine != null) StopCoroutine(interModeCoroutine);

        if (AutoBossChallenge)
        {
            UpdateCurrentStageData();

            isOnTimer = true;
            interModeCoroutine = StartCoroutine(InitBossStage());
            timerCoroutine = StartCoroutine(Timer(currentStageData.BossTimeLimit, BossModeFailed));
        }
        else
        {
            interModeCoroutine = StartCoroutine(InitNormalState());
        }
    }

    private void BossModeFailed()
    {
        UIManager.instance.TryGetUI<UIStageFail>().ShowUI();

        ManageBossCallback(false);
        SetBossModeValidity(true);

        if (interModeCoroutine != null) StopCoroutine(interModeCoroutine);
        interModeCoroutine = StartCoroutine(InitNormalState());
    }

    private void ManageBossCallback(bool isAdding)
    {
        if (isAdding)
        {
            // currentBoss.dataController.GetData(Defines.EDataType.CurrentHealth).OnDataChange += UpdateBossHealth;
            currentBoss.controller.onCurrentHPChange += UpdateBossCurrentHp;
        }
        else
        {
            currentBoss.controller.onCurrentHPChange -= UpdateBossCurrentHp;
        }
    }

    private void UpdateBossCurrentHp(BigInteger health, BigInteger max)
    {
        OnBossHPChange?.Invoke(health, max);
    }

    private void SetBossModeValidity(bool isAvailable)
    {
        isBossStageAvailable = isAvailable;
        OnBossStageValidityChange?.Invoke(isBossStageAvailable);
    }

    // public void GetStageInfo()
    // {
    //     string stageName =
    //         $"{difficultyDic[currentDifficulty]}  {currentStageData.StageSection} - {currentStageData.StageNumber}";
    //     OnStageChange?.Invoke(stageName);
    // }

    IEnumerator EnterDungeon(DungeonData data)
    {
        state = Defines.EStageState.Inter;
        OnStateChange?.Invoke(state);

        player.Wait();
        player.health.Resurrection();
        player.transform.position = Vector3.zero;
        QuestManager.instance.StopCounter(EAchievementType.KillCount);
        UIManager.instance.TryGetUI<UIQuestBar>().HideUI();
        SetDungeonData(data);
        
        // TODO 화면 어두워지게 만들기
        switch (data.dungeonType)
        {
            case EDungeonType.Awaken:
            case EDungeonType.Gold:
                yield return new WaitForSeconds(.5f);
                break;
            case EDungeonType.Enhance:
                yield return new WaitForSeconds(currentBoss.spawnAnimTime);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        player.Idle();
    }

    IEnumerator InitNormalState(Action onSet = null)
    {
        state = Defines.EStageState.Inter;
        OnStateChange?.Invoke(state);

        player.Wait();
        player.health.Resurrection();
        player.transform.position = Vector3.zero;
        QuestManager.instance.RestartCounter(EAchievementType.KillCount);
        UIManager.instance.TryGetUI<UIQuestBar>().DisplayUI();

        // TODO 화면 어두워지게 만들기
        yield return new WaitForSeconds(.5f);

        onSet?.Invoke();

        UpdateCurrentStageData();
        monsterSpawner.StartInfiniteGenerating();
        player.controller.onDeathStart += UIManager.instance.TryGetUI<UIStageFail>().ShowUI;
        string stageName =
            new StringBuilder().Append(currentStageData.StageName)
                .Append(" ")
                .Append((stageDatas.Length * (currentSection - 1) + currentStage + 1).ToString())
                .Append("-")
                .Append(currentStageData.StageNumber.ToString())
                .ToString();
        var ui = UIManager.instance.TryGetUI<UIStageBar>();
        ui.ShowNormalStageUI(stageName);

        player.Idle();
    }

    IEnumerator InitPlayerResurrectionState()
    {
        state = Defines.EStageState.Inter;
        OnStateChange?.Invoke(state);

        player.transform.position = Vector3.zero;
        QuestManager.instance.RestartCounter(EAchievementType.KillCount);
        UIManager.instance.TryGetUI<UIQuestBar>().DisplayUI();

        yield return new WaitForSeconds(2f);

        UpdateCurrentStageData();
        player.controller.onDeathStart += UIManager.instance.TryGetUI<UIStageFail>().ShowUI;
        monsterSpawner.StartInfiniteGenerating();

        string stageName =
            new StringBuilder().Append(currentStageData.StageName)
                .Append(" ")
                .Append((stageDatas.Length * (currentSection - 1) + currentStage + 1).ToString())
                .Append("-")
                .Append(currentStageData.StageNumber.ToString())
                .ToString();
        var ui = UIManager.instance.TryGetUI<UIStageBar>();
        ui.ShowNormalStageUI(stageName);
        player.Spawn();
    }

    IEnumerator InitBossStage()
    {
        state = Defines.EStageState.Inter;
        OnStateChange?.Invoke(state);

        player.Wait();
        player.health.Resurrection();
        player.transform.position = Vector3.zero;

        currentBoss = monsterSpawner.InstantiateBoss();
        currentBoss.controller.onDeathEnd += () => BossModeSuccess(currentBoss);

        var ui = UIManager.instance.TryGetUI<UIStageBar>();
        ui.ShowProgressUI(EStageState.Boss,
            new StringBuilder().Append((stageDatas.Length * (currentSection - 1) + currentStage + 1).ToString())
                .Append("-")
                .Append(currentStageData.StageNumber.ToString())
                .Append(" ")
                .Append(currentStageData.BossName)
                .ToString(), currentStageData.BossDescription,
            EIconType.Heart, EIconType.Clock);
        OnBossHPChange += ui.UpdateHealth;
        ui.InitUpSlider(ui.upSliderSize, ui.upSliderSize);
        ui.InitDownSlider();
        // ui.InitDownSlider(currentStageData.BossTimeLimit, currentStageData.BossTimeLimit);
        OnTimerChanged += ui.UpdateTimer;

        ManageBossCallback(true);
        player.controller.onDeathStart += UIManager.instance.TryGetUI<UIStageFail>().ShowUI;
        currentBoss.Wait();

        yield return new WaitForSeconds(.5f);

        state = Defines.EStageState.Boss;
        OnStateChange?.Invoke(state);

        player.Idle();
        currentBoss.health.Resurrection();
        currentBoss.Idle();
    }

    IEnumerator Timer(float limitTime, Action onFailed)
    {
        float t = limitTime;

        yield return new WaitForSeconds(.5f);

        while (isOnTimer)
        {
            if (t > 0 && !player.IsDead)
            {
                t -= Time.deltaTime;
                OnTimerChanged?.Invoke(t, limitTime);
            }
            else
            {
                isOnTimer = false;
                onFailed?.Invoke();
            }

            yield return null;
        }
    }

    public GameObject TryGetTarget()
    {
        GameObject nearest = null;
        foreach (var monster in monsterSpawner.monstersOnField)
        {
            if (monster.IsDead || !monster.gameObject.activeInHierarchy)
                continue;
            if (ReferenceEquals(nearest, null) ||
                Vector2.Distance(nearest.transform.position, player.transform.position)
                > Vector2.Distance(monster.transform.position, player.transform.position))
                nearest = monster.gameObject;
        }

        return nearest;
    }

    public MonsterData[] GetTargets()
    {
        var monsters = monsterSpawner.monstersOnField;
        List<MonsterData> targets = new List<MonsterData>();
        foreach (var monster in monsters)
        {
            if (!monster.IsDead && monster.gameObject.activeInHierarchy)
                targets.Add(monster);
        }

        // foreach (var monster in removeTarget)
        // {
        //     monsters.Remove(monster);
        // }

        return targets.ToArray();
    }

    public void InitToDungeon(DungeonData data)
    {
        monsterSpawner.StopGenerating();
        monsterSpawner.ClearMonsters();

        if (interModeCoroutine != null) StopCoroutine(interModeCoroutine);
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);

        isOnTimer = true;
        interModeCoroutine = StartCoroutine(EnterDungeon(data));
        timerCoroutine = StartCoroutine(Timer(data.goalTime, DungeonFailed));
    }

    private void DungeonFailed()
    {
        CameraController.SetFollow(player.gameObject);
        // bottom button control
        var ui = UIManager.instance.TryGetUI<UIBottomMenuCtrl>();
        ui.ActivateDungeonBtn(false);
        // Show failed UI
        UIManager.instance.TryGetUI<UIStageFail>().ShowUI();

        monsterSpawner.StopGenerating();
        monsterSpawner.ClearMonsters();

        if (interModeCoroutine != null) StopCoroutine(interModeCoroutine);
        interModeCoroutine = StartCoroutine(InitNormalState());
    }

    private void InitDungeonForResurrection()
    {
        CameraController.SetFollow(player.gameObject);
        var ui = UIManager.instance.TryGetUI<UIBottomMenuCtrl>();
        ui.ActivateDungeonBtn(false);

        UIManager.instance.TryGetUI<UIStageFail>().ShowUI();

        monsterSpawner.StopGenerating();
        monsterSpawner.ClearMonsters();

        if (interModeCoroutine != null) StopCoroutine(interModeCoroutine);
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);

        interModeCoroutine = StartCoroutine(InitPlayerResurrectionState());
    }

    public void RetireBoss()
    {
        CameraController.SetFollow(player.gameObject);
        AutoBossChallenge = false;

        monsterSpawner.StopGenerating();
        monsterSpawner.ClearMonsters();

        if (interModeCoroutine != null) StopCoroutine(interModeCoroutine);
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);

        interModeCoroutine = StartCoroutine(InitNormalState());
    }

    public void RetireDungeon()
    {
        CameraController.SetFollow(player.gameObject);
        // TODO bottom button control
        var ui = UIManager.instance.TryGetUI<UIBottomMenuCtrl>();
        ui.ActivateDungeonBtn(false);

        monsterSpawner.StopGenerating();
        monsterSpawner.ClearMonsters();

        if (interModeCoroutine != null) StopCoroutine(interModeCoroutine);
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);

        // TODO Show dungeon UI

        interModeCoroutine = StartCoroutine(InitNormalState());
    }

    public void ClearSubscribe()
    {
        // OnMonsterKill -= SwitchToBossStage;
        // OnMonsterKill -= SwitchToNormalStage;
        player.controller.onDeathStart -= InitStageForResurrection;
        player.controller.onDeathStart -= InitDungeonForResurrection;
        var ui = UIManager.instance.TryGetUI<UIStageBar>();
        OnMonsterKill -= ui.UpdateHealth;
        OnTimerChanged -= ui.UpdateTimer;
        OnBossHPChange -= ui.UpdateHealth;
        OnMonsterKill -= SwitchToDungeonReward;
        player.controller.onDeathStart -= UIManager.instance.TryGetUI<UIStageFail>().ShowUI;
    }

    public void SwitchToReward(DungeonData data)
    {
        isOnTimer = false;
        CurrencyManager.instance.SubtractCurrency(data.invitationType, 1);

        StopCoroutine(timerCoroutine);
        if (interModeCoroutine != null) StopCoroutine(interModeCoroutine);
        interModeCoroutine = StartCoroutine(InitNormalState(() => OpenDungeonUI(data)));

        GameManager.instance.GetReward((EQuestRewardType)data.rewardType, data.GetTotalReward());
        CurrencyManager.instance.SaveCurrencies();

        var ui = UIManager.instance.TryGetUI<UIDungeonRewardPanel>();
        string title = $"{Strings.currencyToKOR[(int)data.rewardType]} 획득";
        string instruction =
            $"{data.dungeonSubName} {CustomText.SetColor($"Lv.{data.dungeonLevel}", EColorType.Green)} 클리어!";
        string rewardAmount = data.GetTotalReward().ChangeToShort();
        ui.ShowUI(title, instruction, CurrencyManager.instance.GetIcon((ECurrencyType)data.rewardType), rewardAmount,
            clearPanelTime);

        data.LevelUp();
    }

    private void OpenDungeonUI(DungeonData data)
    {
        var ui1 = UIManager.instance.TryGetUI<UIDungeonPanel>();
        var ui2 = UIManager.instance.TryGetUI<UIDungeonElementPopup>();

        ui1.ShowUI();
        ui2.ShowUI(ui1, data);

        // TODO bottom button control
        UIManager.instance.TryGetUI<UIBottomMenuCtrl>()?.ActivateDungeonBtn(false);
    }

    private void UpdateMaxStage(int currentStage, int currentSection, int currentNumber)
    {
        if (currentStage > maxClearStage || currentSection > maxClearSection || currentNumber > maxClearNumber)
        {
            maxClearStage = currentStage;
            maxClearSection = currentSection;
            maxClearNumber = currentNumber;
            onMaxClearStageChanged?.Invoke(20 * maxClearStage * maxClearSection + maxClearNumber);
            Save();
        }
    }

    public void Save()
    {
        DataManager.Instance.Save(nameof(maxClearStage), maxClearStage);
        DataManager.Instance.Save(nameof(maxClearSection), maxClearSection);
        DataManager.Instance.Save(nameof(maxClearNumber), maxClearNumber);
    }

    public void Load()
    {
        maxClearStage = DataManager.Instance.Load(nameof(maxClearStage), 0);
        maxClearSection = DataManager.Instance.Load(nameof(maxClearSection), 1);
        maxClearNumber = DataManager.Instance.Load(nameof(maxClearNumber), 1);
    }

    public List<Reward> GetCurrentReward(int killCount)
    {
        List<Reward> ret = new List<Reward>();
        int targetLevel = 140 * (maxClearSection - 1) + 20 * (maxClearStage) + maxClearNumber;
        foreach (var reward in stageDatas[currentStage].BasicMonstersReward)
        {
            reward.SetLevel(targetLevel);
            ret.Add(new Reward((ENormalRewardType)reward.rewardType, killCount * reward.straightRewardAmount));
        }

        return ret;
    }
}