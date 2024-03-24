using Defines;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Toggle = UnityEngine.UI.Toggle;

public class UIGrowthPanel : UIPanel
{
    [SerializeField] private Toggle[] bottomToggles;
    [SerializeField] private ToggleGroup bottomToggleGroup;

    // 훈련 => 골드 혹은 다이아를 사용해서 올릴 수 있는 능력치
    [Header("훈련")] [SerializeField] private UIStatusBar statBarPrefab;
    private CustomPool<UIStatusBar> statPool;
    [SerializeField] int statBarPoolSize;
    [SerializeField] private GameObject[] statUis;
    [SerializeField] private RectTransform statRoot;

    // 각성 => 던전에서 나오는 재화로만 올릴 수 있는 능력치
    [Header("각성")] [SerializeField] private UIAwakenBar awakenBarPrefab;
    private CustomPool<UIAwakenBar> awakenPool;
    [SerializeField] private int awakenPoolSize;
    [SerializeField] private GameObject[] awakenUis;
    [SerializeField] private RectTransform awakenRoot;

    // 어빌리티 => 일단 던전 재화로 올릴 수 있게 함
    [Header("어빌리티")] [SerializeField] private UIAbilityBar abilityPrefab;
    private CustomPool<UIAbilityBar> abilityPool;
    [SerializeField] int abilityPoolSize;
    [SerializeField] private GameObject[] abilityUIs;
    [SerializeField] private RectTransform abilityRoot;

    [Header("Currency")]
    public UICurrencyUpdater currencyUI;
    private ETrainingType currentTab;

    [SerializeField] private Transform questGuide;
    [SerializeField] private Transform attackQuestRoot;
    [SerializeField] private Transform healthQuestRoot;
    [SerializeField] private Transform awakenAttackQuestRoot;
    [SerializeField] private Transform awakenDamageReductionQuestRoot;
    [SerializeField] private Transform awakenCriticalChanceQuestRoot;
    [SerializeField] private Transform awakenCriticalDamageQuestRoot;
    [SerializeField] private Transform awakenAttackSpeedQuestRoot;
    [SerializeField] private Transform awakenSkillMultiplierQuestRoot;

    public override UIBase InitUI(UIBase parent)
    {
        base.InitUI(parent);

        statPool = EasyUIPooling.MakePool(statBarPrefab, statRoot,
            x => x.actOnCallback += () => statPool.Release(x),
            x => x.transform.SetAsLastSibling(),
            null, statBarPoolSize, true);

        awakenPool = EasyUIPooling.MakePool(awakenBarPrefab, awakenRoot,
            x => x.actOnCallback += () => awakenPool.Release(x),
            x => x.transform.SetAsLastSibling(),
            null, awakenPoolSize, true);

        abilityPool = EasyUIPooling.MakePool(abilityPrefab, abilityRoot,
            x => x.actOnCallback += () => abilityPool.Release(x),
            x => x.transform.SetAsLastSibling(),
            null, abilityPoolSize, true);

        currencyUI.InitUI(this);
        return this;
    }

    public override void ShowUI()
    {
        base.ShowUI();
        OpenTab(currentTab);
        currencyUI.ShowUI();
    }

    public override void CloseUI()
    {
        base.CloseUI();
        currencyUI.CloseUI();
        CloseTab(currentTab);
    }

    protected override void InitializeBtns()
    {
        base.InitializeBtns();

        for (int i = 0; i < bottomToggles.Length; ++i)
        {
            ETrainingType type = (ETrainingType)i;
            bottomToggles[i].onValueChanged.AddListener((x) =>
            {
                if (x) OpenTab(type);
                else CloseTab(type);
            });
        }
    }

    private void ControlUICurrency(ECurrencyType type)
    {
        currencyUI.ShowCurrency(ECurrencyType.Gold, type == ECurrencyType.Gold);
        currencyUI.ShowCurrency(ECurrencyType.AwakenStone, type == ECurrencyType.AwakenStone);
    }

    public void ChangeTab(ETrainingType type)
    {
        bottomToggles[(int)type].isOn = true;
    }

    private void OpenTab(ETrainingType type)
    {
        CloseTab(currentTab);
        currentTab = type;
        switch (type)
        {
            case ETrainingType.Normal:
                foreach (var ui in statUis)
                {
                    ui.SetActive(true);
                }

                foreach (var item in UpgradeManager.instance.statUpgradeInfo)
                {
                    var obj = statPool.Get();
                    obj.ShowUI(item);
                    if (item.statusType == EStatusType.ATK) attackQuestRoot = obj.GetButtonRect();
                    else if (item.statusType == EStatusType.HP) healthQuestRoot = obj.GetButtonRect();
                }

                ControlUICurrency(ECurrencyType.Gold);
                break;
            case ETrainingType.Awaken:
                foreach (var ui in awakenUis)
                {
                    ui.SetActive(true);
                }

                foreach (var item in UpgradeManager.instance.awakenUpgradeInfo)
                {
                    var obj = awakenPool.Get();
                    obj.ShowUI(item);
                    if (item.statusType == EStatusType.ATK)
                        awakenAttackQuestRoot = obj.GetButtonRect();
                    else if (item.statusType == EStatusType.DMG_REDU)
                        awakenDamageReductionQuestRoot = obj.GetButtonRect();
                    else if (item.statusType == EStatusType.CRIT_CH)
                        awakenCriticalChanceQuestRoot = obj.GetButtonRect();
                    else if (item.statusType == EStatusType.CRIT_DMG)
                        awakenCriticalDamageQuestRoot = obj.GetButtonRect();
                    else if (item.statusType == EStatusType.ATK_SPD)
                        awakenAttackSpeedQuestRoot = obj.GetButtonRect();
                    else if (item.statusType == EStatusType.SKILL_DMG)
                        awakenSkillMultiplierQuestRoot = obj.GetButtonRect();
                }

                ControlUICurrency(ECurrencyType.AwakenStone);
                break;
            // case ETrainingType.Speciality:
            //     foreach (var ui in specialityUis)
            //     {
            //         ui.SetActive(true);
            //     }
            //     foreach (var item in UpgradeManager.instance.specialityUpgradeInfo)
            //     {
            //         var obj = specialityPool.Get();
            //         obj.ShowUI(item);
            //     }
            //     break;
        }
    }

    private void CloseTab(ETrainingType type)
    {
        switch (type)
        {
            case ETrainingType.Normal:
                statPool.Clear();
                break;
            case ETrainingType.Awaken:
                awakenPool.Clear();
                break;
            // case ETrainingType.Speciality:
            //     CloseTab(specialityOpenedUi, specialityUis, specialityPool);
            //     break;
        }
    }

    public override void ShowQuestRoot(EAchievementType type)
    {
        switch (type)
        {
            case EAchievementType.AttackUpgradeCount:
                questGuide.SetParent(attackQuestRoot);
                questGuide.localPosition = Vector3.zero;
                break;
            case EAchievementType.HealthUpgradeCount:
                questGuide.SetParent(healthQuestRoot);
                questGuide.localPosition = Vector3.zero;
                break;
            case EAchievementType.DestinyGem:
                questGuide.SetParent(awakenCriticalChanceQuestRoot);
                questGuide.localPosition = Vector3.zero;
                break;
            case EAchievementType.TempestGem:
                questGuide.SetParent(awakenCriticalDamageQuestRoot);
                questGuide.localPosition = Vector3.zero;
                break;
            case EAchievementType.LightningGem:
                questGuide.SetParent(awakenAttackQuestRoot);
                questGuide.localPosition = Vector3.zero;
                break;
            case EAchievementType.GuardianGem:
                questGuide.SetParent(awakenDamageReductionQuestRoot);
                questGuide.localPosition = Vector3.zero;
                break;
            case EAchievementType.RageGem:
                questGuide.SetParent(awakenAttackSpeedQuestRoot);
                questGuide.localPosition = Vector3.zero;
                break;
            case EAchievementType.AbyssGem:
                questGuide.SetParent(awakenSkillMultiplierQuestRoot);
                questGuide.localPosition = Vector3.zero;
                break;
        }
        questGuide.gameObject.SetActive(true);
        QuestManager.instance.currentQuest.onComplete += x => questGuide.gameObject.SetActive(false);
    }
}