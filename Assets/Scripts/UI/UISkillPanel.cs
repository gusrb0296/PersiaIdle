using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Defines;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UISkillPanel : UIPanel
{
    [Header("Skill List")] public ToggleGroup toggleGroup;
    public UISkillIcon uiSkillIconPrefab;
    public RectTransform root;
    public LinkedList<UISkillIcon> pool;

    private BaseSkillData selected;

    [Header("Equip Popup UI")] public UISkillEquip equipUI;

    [Header("Buttons")]
    public Button equipBtn;

    public Button unequipBtn;
    
    public HoldCheckerButton enhanceBtn;
    public Button exceedBtn;
    public Toggle[] sortingToggles;

    [Header("UI show selected skill")]
    // 이미지
    [SerializeField] private Image skillImage;

    [SerializeField] private Image skillFrame;

    // 개수
    [SerializeField] private TMP_Text count;

    // 개수 슬라이더
    [SerializeField] private Slider countSlider;

    // 레벨
    [SerializeField] private TMP_Text level;

    // skill name
    [SerializeField] private TMP_Text skillName;

    // 레어도
    [SerializeField] private TMP_Text rarity;

    // 설명
    [SerializeField] private TMP_Text description;

    // skill type
    [SerializeField] private TMP_Text skillType;

    // mans consume amount
    [SerializeField] private TMP_Text manaConsumeAmount;

    [Header("Quest")] [SerializeField] private Transform questGuide;
    private Transform equipQuestRoot;
    private Transform enhanceQuestRoot;

    private Action<BaseSkillData> onSelect;
    private Action<int> onSortingToggle;
    private int selectedSorting;

    protected void Awake()
    {
        pool = new LinkedList<UISkillIcon>();
    }

    public override void ShowUI()
    {
        base.ShowUI();
        equipQuestRoot = null;

        ShowAllSkill();
    }

    private void ShowAllSkill()
    {
        var icon = pool.First;
        icon = ShowSkillLoop(icon, SkillManager.instance.GetSkillsOnRarity(ERarity.Common));
        icon = ShowSkillLoop(icon, SkillManager.instance.GetSkillsOnRarity(ERarity.Uncommon));
        icon = ShowSkillLoop(icon, SkillManager.instance.GetSkillsOnRarity(ERarity.Rare));
        icon = ShowSkillLoop(icon, SkillManager.instance.GetSkillsOnRarity(ERarity.Epic));
    }


    private LinkedListNode<UISkillIcon> ShowSkill<T>(LinkedListNode<UISkillIcon> node, T skillData)
        where T : BaseSkillData
    {
        if (ReferenceEquals(node, null))
        {
            var icon = Instantiate(uiSkillIconPrefab, root);
            icon.ShowUI(skillData, this);
            icon.toggle.onValueChanged.AddListener((onoff) =>
            {
                if (onoff)
                {
                    ShowSelected(icon.GetData());
                }
            });
            if (ReferenceEquals(equipQuestRoot, null))
            {
                if (skillData.isOwned && (skillData is AnimSkillData { isEquipped: false }))
                    equipQuestRoot = icon.transform;
            }

            if (ReferenceEquals(enhanceQuestRoot, null))
            {
                if (skillData.isOwned && skillData.IsCanLevelUp())
                    enhanceQuestRoot = icon.transform;
            }

            return pool.AddLast(icon).Next;
        }
        else
        {
            node.Value.ShowUI(skillData, this);
            if (ReferenceEquals(equipQuestRoot, null))
            {
                if (skillData.isOwned)
                    equipQuestRoot = node.Value.transform;
            }

            if (ReferenceEquals(enhanceQuestRoot, null))
            {
                if (skillData.isOwned && skillData.IsCanLevelUp())
                    enhanceQuestRoot = node.Value.transform;
            }

            return node.Next;
        }
    }

    private LinkedListNode<UISkillIcon> ShowSkillLoop<T>(LinkedListNode<UISkillIcon> node, List<T> skillDatas)
        where T : BaseSkillData
    {
        foreach (var skill in skillDatas)
            node = ShowSkill(node, skill);
        return node;
    }

    protected override void InitializeBtns()
    {
        base.InitializeBtns();

        equipBtn.onClick.AddListener(EquipBtn);
        unequipBtn.onClick.AddListener(UnEquipBtn);
        sortingToggles[0].onValueChanged.AddListener((onoff) =>
        {
            if (onoff)
            {
                RemoveSkillIcon();
                var icon = pool.First;
                for (int i = 0; i <= (int)ERarity.Epic; ++i)
                {
                    icon = ShowSkillLoop(icon,
                        SkillManager.instance.GetSkillsOnRarity((ERarity)i)
                            .Where(skill => skill.skillType != ESkillType.Passive).ToList());
                }
                
                selectedSorting = 0;
                onSortingToggle?.Invoke(0);
            }
        });
        sortingToggles[1].onValueChanged.AddListener((onoff) =>
        {
            if (onoff)
            {
                RemoveSkillIcon();
                var icon = pool.First;
                for (int i = 0; i <= (int)ERarity.Epic; ++i)
                    icon = ShowSkillLoop(icon,
                        SkillManager.instance.GetSkillsOnRarity((ERarity)i)
                            .Where(skill => skill.skillType == ESkillType.Passive).ToList());

                selectedSorting = 1;
                onSortingToggle?.Invoke(1);
            }
        });
        sortingToggles[2].onValueChanged.AddListener((onoff) =>
        {
            if (onoff)
            {
                RemoveSkillIcon();
                ShowAllSkill();

                selectedSorting = 2;
                onSortingToggle?.Invoke(2);
            }
        });

        equipUI.actOnCallback += UpdateAllSkillIcon;

        enhanceBtn.onClick.AddListener(UpgradeSelectedSkill);
        enhanceBtn.onExit.AddListener(SaveSelectedSkill);

        // TODO Exceed Btn
    }

    private void SaveSelectedSkill()
    {
        selected.Save(ESkillDataType.Level);
        selected.Save(ESkillDataType.Quantity);
        SkillManager.instance.SaveTotalSkillLevelUp();
    }

    private void UpgradeSelectedSkill()
    {
        if (SkillManager.instance.TryLevelOneUp(selected))
        {
            // show level up effect
            UIEffectManager.instance.ShowUpgradeEffect(skillImage.transform);
            // update ui. quantity and level
            UpdateSelectedUI();
            var icon = pool.First;
            while (!ReferenceEquals(icon, null))
            {
                if (icon.Value.GetData().skillName == selected.skillName)
                {
                    icon.Value.UpdateDisplay();
                    break;
                }

                icon = icon.Next;
            }
        }
        else
        {
            // show not enough message
            MessageUIManager.instance.ShowCenterMessage(CustomText.SetColor("스킬", Color.cyan) + " 개수가 부족합니다.");
        }
    }

    private void EquipBtn()
    {
        if (selected is AnimSkillData anim)
        {
            if (questGuide.gameObject.activeInHierarchy)
            {
                questGuide.gameObject.SetActive(false);
            }

            equipUI.ShowUI(anim);
            UpdateAllSkillIcon();
        }
    }

    private void UnEquipBtn()
    {
        var index = Array.FindIndex(PlayerManager.instance.EquippedSkill,
            (x) => x != null && x.skillName == selected.skillName);
        SkillManager.instance.UnequipSkill(index);
        UpdateAllSkillIcon();
    }

    public void ShowSelected<T>(T skillData) where T : BaseSkillData
    {
        StringBuilder sb = new StringBuilder();

        selected = skillData;

        // show information of skill
        skillImage.sprite = SkillManager.instance.GetIcon(selected.iconIndex);
        skillFrame.sprite = EquipmentManager.instance.GetFrame(selected.rarity);
        rarity.text = Strings.rareKor[(int)selected.rarity];
        
        UpdateSelectedUI();

        skillName.text = selected.skillName;

        skillType.text = Strings.skillTypeToKor[(int)selected.skillType];
        if (selected is AnimSkillData anim)
        {
            manaConsumeAmount.gameObject.SetActive(true);

            equipBtn.gameObject.SetActive(!anim.isEquipped);
            unequipBtn.gameObject.SetActive(anim.isEquipped);

            equipBtn.interactable = selected.isOwned;
            unequipBtn.interactable = selected.isOwned;
        }
        else
        {
            manaConsumeAmount.text = "";

            equipBtn.gameObject.SetActive(false);
            unequipBtn.gameObject.SetActive(false);
        }

        onSelect?.Invoke(selected);
    }

    public void UpdateSelectedUI()
    {
        StringBuilder sb = new StringBuilder();

        count.text = sb.Clear().Append(selected.quantity).Append(" / ").Append(selected.QuantityToLevelUp()).ToString();
        countSlider.maxValue = selected.QuantityToLevelUp();
        countSlider.value = Mathf.Clamp(selected.quantity, 0, selected.QuantityToLevelUp());

        level.text =
            CustomText.SetColor(sb.Clear().Append("(Lv.").Append(selected.levelFrom0).Append(" / 100)").ToString(),
                Color.yellow);

        sb.Clear().Append(selected.descriptions[0])
            .Append(CustomText.SetColor(selected.GetDescriptionVariable(0), EColorType.Green))
            .Append(selected.descriptions[1]);
        if (selected.descriptions.Length > 2)
        {
            sb.Append(CustomText.SetColor(selected.GetDescriptionVariable(1), EColorType.Green))
                .Append(selected.descriptions[2]);
        }

        description.text = sb.ToString();

        if (selected is AnimSkillData anim)
        {
            manaConsumeAmount.text = sb.Clear().Append("마나 소모량 : ")
                .Append(CustomText.SetColor(anim.ManaConsume.ToString(), Color.cyan))
                .ToString();
        }
    }

    private void UpdateAllSkillIcon()
    {
        var icon = pool.First;
        while (!ReferenceEquals(icon, null))
        {
            icon.Value.UpdateDisplay();
            icon = icon.Next;
        }

        if (selected is AnimSkillData anim)
        {
            equipBtn.gameObject.SetActive(!anim.isEquipped);
            unequipBtn.gameObject.SetActive(anim.isEquipped);
        }

        equipBtn.interactable = selected.isOwned;
        unequipBtn.interactable = selected.isOwned;
    }

    private void RemoveSkillIcon()
    {
        var icon = pool.First;
        while (!ReferenceEquals(icon, null))
        {
            icon.Value.CloseUI();
            icon = icon.Next;
        }
    }

    public override void CloseUI()
    {
        base.CloseUI();
        RemoveSkillIcon();
    }

    public override void ShowQuestRoot(EAchievementType type)
    {
        if (type == EAchievementType.SkillEquip)
        {
            if (ReferenceEquals(equipQuestRoot, null)) return;
            questGuide.SetParent(equipQuestRoot);
            questGuide.position = equipQuestRoot.position;
            equipUI.ShowQuestRoot(type);
            onSelect = (skill) =>
            {
                if (selectedSorting == 1)
                {
                    questGuide.SetParent(sortingToggles[0].transform);
                    questGuide.position = sortingToggles[0].transform.position;
                }
                else
                {
                    if (skill.isOwned && (selected is AnimSkillData { isEquipped: false }))
                    {
                        questGuide.SetParent(transform);
                        questGuide.position = equipBtn.transform.position;
                    }
                    else
                    {
                        questGuide.SetParent(equipQuestRoot);
                        questGuide.position = equipQuestRoot.position;
                    }
                }
            };
            onSortingToggle = (toggle) =>
            {
                if (toggle is 0 or 2)
                {
                    if (ReferenceEquals(equipQuestRoot, null)) return;
                    questGuide.SetParent(equipQuestRoot);
                    questGuide.position = equipQuestRoot.position;
                }
            };
        }
        else if (type == EAchievementType.SkillLevelUp)
        {
            if (ReferenceEquals(enhanceQuestRoot, null)) return;
            questGuide.SetParent(enhanceQuestRoot);
            questGuide.position = enhanceQuestRoot.position;
            onSelect = (skill) =>
            {
                if (skill.IsCanLevelUp())
                {
                    questGuide.SetParent(enhanceBtn.transform);
                    questGuide.position = enhanceBtn.transform.position;
                }
                else
                {
                    if (ReferenceEquals(enhanceQuestRoot, null)) return;
                    questGuide.SetParent(enhanceQuestRoot);
                    questGuide.position = enhanceQuestRoot.position;
                }
            };
        }

        questGuide.gameObject.SetActive(true);
        QuestManager.instance.currentQuest.onComplete += x => questGuide.gameObject.SetActive(false);
    }
}