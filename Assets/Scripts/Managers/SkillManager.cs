using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using Keiwando.BigInteger;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using Utils;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    [Header("지정 필요")] 
    public SkillSystem[] playerActiveSkillPrefabs;
    public ActiveSkillFixedInfo[] activeSkillFixedInfos;
    public BuffSkillFixedInfo[] buffSkillFixedInfos;
    public PassiveSkillFixedInfo[] passiveSkillFixedInfos;
    public Sprite[] skillIcon;

    [Header("특수기 전용")] 
    public SpecialSkillData playerSpecialSkillData;
    public ActiveSkillFixedInfo playerSpecialActiveSkillInfo;

    [Header("추출 확인용")] 
    public ActiveSkillData[] playerActiveSkillDatas;
    public BuffSkillData[] playerBuffSkillDatas;
    public PassiveSkillData[] playerPassiveSkillData;

    protected Dictionary<string, SkillSystem> animSkillSystems;
    protected Dictionary<string, AnimSkillData> animSkillData;
    protected Dictionary<ERarity, List<BaseSkillData>> skillPerRarity;

    // TODO Timer 바꾸기
    protected Dictionary<string, float> timer;

    protected float specialTimer;
    public event Action<float, float> onSpecialTimer;
    // public event Action<int, float, float> onSkillTimer;

    private UISkillSlot uiSkillSlot;

    private Coroutine skillTimerCoroutine;

    public event Action<bool> onAutoSkill;

    public event Action<BigInteger> onSkillLevelUpTotal;
    private BigInteger totalSkillLevelUp;
    public BigInteger TotalSkilllevelUp => totalSkillLevelUp;
    public bool IsAutoSkill
    {
        get => isAutoSkill;
        set
        {
            if (isAutoSkill != value)
            {
                SaveAuto(value);
            }

            isAutoSkill = value;
            onAutoSkill?.Invoke(value);
        }
    }

    private bool isAutoSkill;

    private void Start()
    {
        skillTimerCoroutine = StartCoroutine(SkillTimer());
    }

    public List<BaseSkillData> GetSkillsOnRarity(ERarity rare)
    {
        return skillPerRarity[rare];
    }

    public int GetSkillOwnCount(ESkillType type)
    {
        int count = 0;
        switch (type)
        {
            case ESkillType.Active:
                foreach (var skill in playerActiveSkillDatas)
                {
                    if (skill.isOwned) ++count;
                }

                break;
            case ESkillType.Buff:
                foreach (var skill in playerBuffSkillDatas)
                {
                    if (skill.isOwned) ++count;
                }

                break;
            case ESkillType.Passive:
                foreach (var skill in playerPassiveSkillData)
                {
                    if (skill.isOwned) ++count;
                }

                break;
        }

        return count;
    }

    private void Awake()
    {
        instance = this;
        animSkillSystems = new Dictionary<string, SkillSystem>();
        animSkillData = new Dictionary<string, AnimSkillData>();
        skillPerRarity = new Dictionary<ERarity, List<BaseSkillData>>();
        for (int i = 0; i < Enum.GetNames(typeof(ERarity)).Length; ++i)
            skillPerRarity.Add((ERarity)i, new List<BaseSkillData>());

        timer = new Dictionary<string, float>();
    }

    private IEnumerator SkillTimer()
    {
        while (true)
        {
            if (specialTimer > 0)
            {
                specialTimer -= Time.deltaTime;
                CallUpdateSpecialSkillTimer(specialTimer);
            }

            foreach (var (skillName, data) in animSkillData)
            {
                var t = timer[skillName];

                if (t > 0)
                {
                    var restT = t - Time.deltaTime;
                    timer[skillName] = restT;

                    if (PlayerManager.instance.skillNamtToSlot.TryGetValue(skillName, out int slot))
                    {
                        // onSkillTimer?.Invoke(slot, restT, allSkillDatas[skillName].coolTime);
                        var ui = UIManager.instance.TryGetUI<UISkillSlot>();
                        ui.UpdateTimer(slot, restT, animSkillData[skillName].coolTime);
                    }
                }
                else
                {
                    if (IsAutoSkill && PlayerManager.instance.skillNamtToSlot.TryGetValue(skillName, out int slot))
                    {
                        if (PlayerManager.instance.CheckStateCanUseSkill())
                            PlayerManager.instance.UseSkill(slot);
                    }
                    // if (isAutoSkill && allSkillDatas[skillName].isEquipped)
                    // {
                    //     // if (PlayerManager.instance.CheckStateCanUseSkill())
                    //     // {
                    //     //     PlayerManager.instance.player.controller.CallSkill(skillName);
                    //     // }
                    // }
                }
            }

            yield return null;
        }
    }

    private void CallUpdateSpecialSkillTimer(float time)
    {
        onSpecialTimer?.Invoke(time, playerSpecialSkillData.coolTime);
    }

    public void InitSkillManager()
    {
        // if (ES3.KeyExists("Init_Game"))
        // {
        //     playerActiveSkillDatas = DataManager.Instance.Load(activeSkillSaveKey, playerActiveSkillDatas);
        //     playerPassiveSkillDatas = DataManager.Instance.Load(passiveSkillSaveKey, playerPassiveSkillDatas);
        // }
        uiSkillSlot = UIManager.instance.TryGetUI<UISkillSlot>();

        foreach (var info in activeSkillFixedInfos)
        {
            Array.Find(playerActiveSkillDatas, x => x.skillName == info.skillName).SetInfo(info);
        }

        foreach (var info in buffSkillFixedInfos)
        {
            Array.Find(playerBuffSkillDatas, x => x.skillName == info.skillName).SetInfo(info);
        }

        foreach (var info in passiveSkillFixedInfos)
        {
            Array.Find(playerPassiveSkillData, x => x.skillName == info.skillName).SetInfo(info);
        }

        Load();
        foreach (var activeSkill in playerActiveSkillDatas)
        {
            animSkillData.TryAdd(activeSkill.skillName, activeSkill);
            skillPerRarity[activeSkill.rarity].Add(activeSkill);
            timer.Add(activeSkill.skillName, 0.0f);
        }

        for (int i = 0; i < playerActiveSkillPrefabs.Length; ++i)
        {
            var skill = Instantiate(playerActiveSkillPrefabs[i]);
            skill.gameObject.SetActive(false);
            skill.InitData();
            skill.InitSkillSystem(PlayerManager.instance.player,
                Array.Find(playerActiveSkillDatas, x => x.skillName == skill.skillName));
            animSkillSystems.TryAdd(skill.skillName, skill);
        }

        foreach (var buffSkill in playerBuffSkillDatas)
        {
            animSkillData.TryAdd(buffSkill.skillName, buffSkill);
            skillPerRarity[buffSkill.rarity].Add(buffSkill);
            if (buffSkill.coolTime > 0)
                timer.Add(buffSkill.skillName, 0.0f);
        }

        foreach (var passiveSkill in playerPassiveSkillData)
        {
            skillPerRarity[passiveSkill.rarity].Add(passiveSkill);
        }

        playerSpecialSkillData.SetInfo(playerSpecialActiveSkillInfo);

        PlayerManager.instance.player.specialSkill.InitSkillSystem(PlayerManager.instance.player,
            playerSpecialSkillData);
    }

    private void Load()
    {
        LoadAuto();

        foreach (var skill in playerActiveSkillDatas)
            skill.Load();
        foreach (var skill in playerBuffSkillDatas)
            skill.Load();
        foreach (var skill in playerPassiveSkillData)
            skill.Load();
        playerSpecialSkillData.Load();
        totalSkillLevelUp = new BigInteger(DataManager.Instance.Load<string>($"{nameof(SkillManager)}_{nameof(totalSkillLevelUp)}", "0"));
    }

    private void LoadAuto()
    {
        isAutoSkill = DataManager.Instance.Load<bool>(nameof(isAutoSkill), false);
        if (isAutoSkill) uiSkillSlot.ShowUI();
    }

    private void SaveAuto(bool value)
    {
        DataManager.Instance.Save<bool>(nameof(isAutoSkill), value);
    }

    public SkillSystem GetSkillSystem(string skillName)
    {
        return animSkillSystems[skillName];
    }

    public bool CallActiveSkill(string skillName, out Vector3 direction)
    {
        if (animSkillData[skillName] is ActiveSkillData active)
        {
            switch (active.attackType)
            {
                case ESkillAttackType.Single:
                    return CallSkillSingle(active.skillName, out direction);
                case ESkillAttackType.Multiple:
                    return CallSkillMultiple(active.skillName, out direction);
            }
        }

        direction = Vector3.zero;
        return false;
    }

    // Search Target and Start Skill
    private bool CallSkillSingle(string skillName, out Vector3 direction)
    {
        if (!PlayerManager.instance.player.health.SubstractMP(animSkillData[skillName].ManaConsume))
        {
            direction = Vector3.zero;
            return false;
        }

        // var target = StageManager.instance.TryGetTarget();
        //
        // if (ReferenceEquals(target, null) || target == null)
        //     direction = Vector3.zero;
        // else
        //     direction = target.transform.position - transform.position;
        direction = Vector3.left * PlayerManager.instance.player.spriteController.horizontalDirection;

        StartSingleSkill(skillName, PlayerManager.instance.player.transform.position, direction);
        return true;
    }

    // Search Targets and Start Skill
    private bool CallSkillMultiple(string skillName, out Vector3 direction)
    {
        var targets = StageManager.instance.GetTargets();
        if (ReferenceEquals(targets, null))
        {
            direction = Vector3.zero;
            return false;
        }

        List<Transform> targetTransforms = new List<Transform>();
        foreach (var target in targets)
        {
            if (!target.IsDead && target.gameObject.activeInHierarchy)
                targetTransforms.Add(target.transform);
        }

        if (targetTransforms.Count > 0)
        {
            if (!PlayerManager.instance.player.health.SubstractMP(animSkillData[skillName].ManaConsume))
            {
                direction = Vector3.zero;
                return false;
            }

            StartMultipleSkill(skillName, targetTransforms.ToArray());
            direction = targetTransforms[0].position;
            return true;
        }
        else
        {
            direction = Vector3.zero;
            return false;
        }
    }

    // Start Skill
    private void StartSingleSkill(string skillName, Vector3 center, Vector3 direction)
    {
        timer[skillName] = animSkillData[skillName].coolTime;

        var skill = GetSkillSystem(skillName);

        skill.transform.position = center;
        // Debug.Log($"skill direction {direction.x}.{direction.y}.{direction.z}");
        var dir = GetLeftOrRight(direction);
        var local = skill.transform.localScale;
        // Debug.Log($"skill dir {dir.x}.{dir.y}.{dir.z}");
        skill.transform.localScale = new Vector3(Mathf.Abs(local.x) * dir.x, local.y * dir.y, local.z * dir.z);

        skill.StartSkill();
    }

    // Start Skill
    private void StartMultipleSkill(string skillName, Transform[] transforms)
    {
        timer[skillName] = animSkillData[skillName].coolTime;

        var skill = GetSkillSystem(skillName);

        skill.StartSkill(transforms);
    }

    public bool CallBuffSkill(string skillName, out float duration)
    {
        duration = .0f;
        if (!PlayerManager.instance.player.health.SubstractMP(animSkillData[skillName].ManaConsume))
            return false;

        if (animSkillData[skillName] is BuffSkillData buff)
        {
            duration = buff.skillFullTime;
            if (buff.coolTime > 0)
                timer[skillName] = buff.coolTime;

            // TODO apply buff
            PlayerManager.instance.AddBuffToList(buff.tempBuffStatus);
            if (buff.coolTime > 0)
                StartCoroutine(BuffDuration(buff.skillFullTime, buff.tempBuffStatus));
            return true;
        }

        return false;
    }

    public bool CanUseEquippedSkill(string skillName)
    {
        Debug.Assert(animSkillData[skillName].isEquipped);
        if (timer[skillName] <= 0)
            return true;
        return false;
    }


    public bool CallSpecial(out float duration)
    {
        Debug.Assert(CanUseSpecial());
        specialTimer = playerSpecialSkillData.coolTime;

        // TODO use Special skill
        duration = playerSpecialSkillData.skillFullTime;

        PlayerManager.instance.AddBuffToList(playerSpecialSkillData.tempBuffStatus);
        StartCoroutine(BuffDuration(playerSpecialSkillData.skillFullTime, playerSpecialSkillData.tempBuffStatus));

        // TODO Active special skill
        PlayerManager.instance.player.specialSkill.StartSkill();

        return true;
    }

    public bool CanUseSpecial()
    {
        if (specialTimer <= 0)
            return true;
        return false;
    }

    IEnumerator BuffDuration(float duration, TempBuffStatus tempBuffStatus)
    {
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            yield return null;
        }

        PlayerManager.instance.RemoveBuffFromList(tempBuffStatus);
    }

    public void EquipSkill(int slot, AnimSkillData target)
    {
        if (PlayerManager.instance.EquippedSkill[slot] != null)
            PlayerManager.instance.UnequipSkill(slot);
        PlayerManager.instance.EquipSkill(slot, target);

        Debug.Assert(uiSkillSlot != null, "[SkillManager] UI Skill Slot not found.");
        uiSkillSlot.ShowUI(slot, target);
        target.SaveEquip(true, slot);
    }

    public void UnequipSkill(int slot)
    {
        PlayerManager.instance.UnequipSkill(slot);

        Debug.Assert(uiSkillSlot != null, "[SkillManager] UI Skill Slot not found.");
        uiSkillSlot.ShowVoidUI(slot);
    }

    // public void UseSkill(string skillName)
    // {
    //     var skillData = allSkillDatas[skillName];
    //
    //     switch (skillData.attackType)
    //     {
    //         case ESkillAttackType.Single:
    //             PlayerManager.instance.player.controller.CallSkillSingle(skillData.skillName);
    //             break;
    //         case ESkillAttackType.Multiple:
    //             PlayerManager.instance.player.controller.CallSkillMultiple(skillData.skillName);
    //             break;
    //     }
    // }

    public AnimSkillData GetSkill(string skillname)
    {
        return animSkillData[skillname];
    }

    public Sprite GetIcon(int skillDataIconIndex)
    {
        return skillIcon[skillDataIconIndex];
    }

    public Vector3 GetLeftOrRight(Vector3 direction)
    {
        var dec = Vector2.Dot(direction, Vector2.right);
        if (dec < 0)
        {
            return new Vector3(1, 1, 1);
        }
        else
        {
            return new Vector3(-1, 1, 1);
        }
    }
    
    // TODO skill level up
    public bool TryLevelOneUp<T>(T skill) where T : BaseSkillData
    {
        if (skill.TryLevelUp())
        {
            // skill.Save(ESkillDataType.Level);
            ++totalSkillLevelUp;
            return true;
        }

        return false;
    }

    public void SaveTotalSkillLevelUp()
    {
        onSkillLevelUpTotal?.Invoke(totalSkillLevelUp);
        DataManager.Instance.Save($"{nameof(SkillManager)}_{nameof(totalSkillLevelUp)}", totalSkillLevelUp.ToString());
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SkillManager))]
public class CustomEditorSkillManager : Editor
{
    private TextAsset csvFile1;
    private TextAsset csvFile2;
    private TextAsset csvFile3;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.BeginHorizontal();

        csvFile1 = EditorGUILayout.ObjectField("액티브 CSV", csvFile1, typeof(TextAsset), true) as TextAsset;
        if (GUILayout.Button("액티브 스킬 추출"))
        {
            LoadActiveSkillFromCSV(csvFile1);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        csvFile2 = EditorGUILayout.ObjectField("버프 CSV", csvFile2, typeof(TextAsset), true) as TextAsset;
        if (GUILayout.Button("버프 스킬 추출"))
        {
            LoadBuffSkillFromCSV(csvFile2);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        csvFile3 = EditorGUILayout.ObjectField("패시브 CSV", csvFile3, typeof(TextAsset), true) as TextAsset;
        if (GUILayout.Button("패시브 스킬 추출"))
        {
            LoadPassiveSkillFromCSV(csvFile3);
        }

        EditorGUILayout.EndHorizontal();
    }

    private void LoadBuffSkillFromCSV(TextAsset csv)
    {
        List<BuffSkillData> buffSkills = new List<BuffSkillData>();

        string[] lines = csv.text.Split('\n');

        for (int i = 1; i < lines.Length; i++) // 첫 번째 줄(헤더) 건너뛰기
        {
            string line = lines[i];
            if (!string.IsNullOrWhiteSpace(line))
            {
                string[] fields = line.Split(',');
                int j = 0;

                // skillName
                string skillName = fields[j].Trim();

                // rarity
                Extract(fields[++j].Trim(), out int rarity);

                // description
                string description = fields[++j].Trim();

                // type
                Extract(fields[++j].Trim(), out int skillType);

                // coolTime
                Extract(fields[++j].Trim(), out float coolTime);

                // mana consume
                Extract(fields[++j].Trim(), out int manaConsume);

                // skill full time
                Extract(fields[++j].Trim(), out float skillFullTime);

                // buffStatus
                // atk
                Extract(fields[++j].Trim(), out int atk);
                // hp
                Extract(fields[++j].Trim(), out int hp);
                // dmg red
                Extract(fields[++j].Trim(), out int dmg_red);
                // mana
                Extract(fields[++j].Trim(), out int mana);
                // mana rec
                Extract(fields[++j].Trim(), out int mana_rec);
                // crit ch
                Extract(fields[++j].Trim(), out int crit_ch);
                // crit dmg
                Extract(fields[++j].Trim(), out int crit_dmg);
                // atk spd
                Extract(fields[++j].Trim(), out int atk_spd);
                // mov spd
                Extract(fields[++j].Trim(), out int mov_spd);
                // skill dmg
                Extract(fields[++j].Trim(), out int skill_dmg);

                TempBuffStatus status =
                    new TempBuffStatus(atk, hp, dmg_red, mana, mana_rec, crit_ch, crit_dmg, atk_spd, mov_spd,
                        skill_dmg);

                BuffSkillData skill = new BuffSkillData(skillName, (ERarity)rarity,
                    description, (ESkillType)skillType, coolTime, manaConsume,
                    skillFullTime, status);

                buffSkills.Add(skill);
            }
        }

        // ES3.Save(nameof(passiveSkills), passiveSkills.ToArray());
        (target as SkillManager).playerBuffSkillDatas = buffSkills.ToArray();
        // serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }

    private void LoadPassiveSkillFromCSV(TextAsset csv)
    {
        List<PassiveSkillData> passiveSkills = new List<PassiveSkillData>();

        string[] lines = csv.text.Split('\n');

        for (int i = 1; i < lines.Length; i++) // 첫 번째 줄(헤더) 건너뛰기
        {
            string line = lines[i];
            if (!string.IsNullOrWhiteSpace(line))
            {
                string[] fields = line.Split(',');
                int j = 0;

                // skillName
                string skillName = fields[j].Trim();

                // rarity
                Extract(fields[++j].Trim(), out int rarity);

                // description
                string description = fields[++j].Trim();

                // type
                Extract(fields[++j].Trim(), out int skillType);

                // target status type
                Extract(fields[++j].Trim(), out int statusType);

                // coolTime
                Extract(fields[++j].Trim(), out int buff);

                PassiveSkillData skill = new PassiveSkillData(skillName, (ERarity)rarity, description,
                    (ESkillType)skillType, (EStatusType)statusType, buff);

                passiveSkills.Add(skill);
            }
        }

        // ES3.Save(nameof(passiveSkills), passiveSkills.ToArray());
        (target as SkillManager).playerPassiveSkillData = passiveSkills.ToArray();
        // serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }

    private void LoadActiveSkillFromCSV(TextAsset csv)
    {
        List<ActiveSkillData> activeSkills = new List<ActiveSkillData>();

        string[] lines = csv.text.Split('\n');

        for (int i = 1; i < lines.Length; i++) // 첫 번째 줄(헤더) 건너뛰기
        {
            string line = lines[i];
            if (!string.IsNullOrWhiteSpace(line))
            {
                string[] fields = line.Split(',');
                int j = 0;

                // skillName
                string skillName = fields[j].Trim();

                // rarity
                Extract(fields[++j].Trim(), out int rarity);

                // description
                string description = fields[++j].Trim();

                // type
                Extract(fields[++j].Trim(), out int skillType);

                // coolTime
                Extract(fields[++j].Trim(), out float coolTime);

                // mana consume
                Extract(fields[++j].Trim(), out int manaConsume);

                // skill full time
                Extract(fields[++j].Trim(), out float skillFullTime);

                // max Attack Count
                Extract(fields[++j].Trim(), out int maxAttackCount);

                // multiplier
                Extract(fields[++j].Trim(), out int multiplier);

                // attackDistance
                Extract(fields[++j].Trim(), out float attackDistance);

                Extract(fields[++j].Trim(), out float tickUnitTime);

                ActiveSkillData skill = new ActiveSkillData(skillName, (ERarity)rarity,
                    description, (ESkillType)skillType, coolTime, manaConsume,
                    skillFullTime, maxAttackCount, multiplier, attackDistance, tickUnitTime);

                activeSkills.Add(skill);
            }
        }

        // ES3.Save(nameof(activeSkills), activeSkills.ToArray());
        (target as SkillManager).playerActiveSkillDatas = activeSkills.ToArray();
        EditorUtility.SetDirty(target);
        // serializedObject.ApplyModifiedProperties();
    }

    private void Extract(string str, out int data)
    {
        var isSuccess = int.TryParse(str, out data);
        Debug.Assert(isSuccess, $"Failed {str}");
    }

    private void Extract(string str, out float data)
    {
        var isSuccess = float.TryParse(str, out data);
        Debug.Assert(isSuccess, $"Failed {str}");
    }
}
#endif