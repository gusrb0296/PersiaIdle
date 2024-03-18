using System.Collections.Generic;
using System.Text;
using Defines;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UIEquipmentPanel : UIPanel
{
    [SerializeField] private Toggle[] equipmentType;
    [SerializeField] private Button equipBtn;
    [SerializeField] private Button autoEquipBtn;
    [SerializeField] private Button compositeBtn;
    [SerializeField] private Button compositeAllBtn;
    [SerializeField] private Button enhanceBtn;

    [SerializeField] private RectTransform root;
    [SerializeField] private UIEquipment equipmentPrefab;
    private CustomPool<UIEquipment> pool;
    [SerializeField] private int poolSize;

    [SerializeField] private ToggleGroup equipmentGroup;

    [Header("Selected Equipments")]
    // 이미지
    [SerializeField] private Image image;
    [SerializeField] private Image background;
    // 색
    [SerializeField] private Image backEffecct;
    // 레벨
    [SerializeField] private TMP_Text level;
    // 개수
    [SerializeField] private TMP_Text count;
    [SerializeField] private Slider countSlider;
    // 레어도
    [SerializeField] private TMP_Text rarity;
    // 보유 효과
    [SerializeField] private TMP_Text ownedEffectTitle;
    [SerializeField] private TMP_Text ownedEffect;
    // 장착 효과
    [SerializeField] private TMP_Text equipEffectTitle;
    [SerializeField] private TMP_Text equipEffect;
    // 각성 효과
    [SerializeField] private TMP_Text awakenEffectTitle;
    [SerializeField] private TMP_Text awakenEffect;
    [SerializeField] private GameObject awakenEffectLock;
    [SerializeField] private UIEnhancePopup uiEnhancePopup;

    private WeaponInfo selectedWeapon;
    private UIEquipment uiSelectedWeapon;
    private ArmorInfo selectedArmor;
    private UIEquipment uiSelectedArmor;
    private EEquipmentType selectedTab;

    [SerializeField] private Transform questGuide;
    [SerializeField] private Transform equipQuestRoot;
    [SerializeField] private Transform compositeQuestRoot;
    [SerializeField] private Transform enhanceQuestRoot;
    
    public override void ShowUI()
    {
        selectedArmor = PlayerManager.instance.EquippedArmor;
        selectedWeapon = PlayerManager.instance.EquippedWeapon;
        base.ShowUI();
        OpenTab(selectedTab);
    }

    public void OpenTab(EEquipmentType type)
    {
        for (int i = 0; i < equipmentType.Length; ++i)
            equipmentType[i].SetIsOnWithoutNotify(i == (int)type);
        
        selectedTab = type;
        
        while (pool.UsedCount > 0)
            pool.UsedList.First.Value.CloseUI();
        pool.Clear();
        
        switch (type)
        {
            case EEquipmentType.Weapon:
                OpenTabElement(EquipmentManager.instance.weapons, selectedWeapon);
                break;
            case EEquipmentType.Armor:
                OpenTabElement(EquipmentManager.instance.armors, selectedArmor);
                break;
        }
        
        compositeAllBtn.interactable = EquipmentManager.instance.CanComposite(selectedTab);
        UpdateSelectedUI(type);
    }

    private void OpenTabElement<T>(List<T> items, T selected) where T : Equipment
    {
        for (int i = 0; i < items.Count; ++i)
        {
            var item = items[i];
            var ui = pool.Get();
            ui.ShowUI(item, this);
            if (item.IsOwned)
            {
                if (ReferenceEquals(item, selected) || ReferenceEquals(selected, null))
                {
                    ui.toggle.isOn = true;
                    SelectEquipment(ui);
                }
                ui.UpdateEquippedMark(item.IsEquipped);
            }
            else
            {
                ui.gameObject.SetActive(false);
                item.onOwned += ui.RevealUI;
                ui.UpdateEquippedMark(item.IsEquipped);
            }
            ui.transform.SetAsLastSibling();
        }
    }

    public override UIBase InitUI(UIBase _parent)
    {
        base.InitUI(_parent);
        pool = EasyUIPooling.MakePool(equipmentPrefab, root, x =>
        {
            x.actOnCallback += () => pool.Release(x);
            x.toggle.group = equipmentGroup;
            x.AddListener((onoff) => { if (onoff) SelectEquipment(x); });
        },
            x => x.transform.SetAsLastSibling(),
            x => x.toggle.isOn = false, poolSize, true);

        return this;
    }

    protected override void InitializeBtns()
    {
        base.InitializeBtns();

        for (int i = 0; i < equipmentType.Length; ++i)
        {
            EEquipmentType type = (EEquipmentType)i;
            equipmentType[i].onValueChanged.AddListener((onoff) =>
            {
                if (onoff)
                {
                    OpenTab(type);
                }
            });
        }

        equipBtn.onClick.AddListener(() =>
        {
            if (selectedTab == EEquipmentType.Weapon)
            {
                PlayerManager.instance.EquipItem(selectedWeapon);
            }
            else if (selectedTab == EEquipmentType.Armor)
            {
                PlayerManager.instance.EquipItem(selectedArmor);
            }
            UpdateSelectedUI(selectedTab);
        });

        autoEquipBtn.onClick.AddListener(() =>
        {
            EquipmentManager.instance.AutoEquip(selectedTab);
            UpdateSelectedUI(selectedTab);
            // TODO : 자동 장착된 아이템의 위치로 UI 이동시키기
            
        });

        compositeBtn.onClick.AddListener(() =>
        {
            if (selectedTab == EEquipmentType.Weapon)
            {
                EquipmentManager.instance.CompositeOnce(selectedWeapon);
            }
            else if (selectedTab == EEquipmentType.Armor)
            {
                EquipmentManager.instance.CompositeOnce(selectedArmor);
            }
            UpdateSelectedUI(selectedTab);

            compositeBtn.interactable = false;
            compositeAllBtn.interactable = EquipmentManager.instance.CanComposite(selectedTab);
        });

        compositeAllBtn.onClick.AddListener(() =>
        {
            EquipmentManager.instance.CompositeAll(selectedTab);
            UpdateSelectedUI(selectedTab);
            
            compositeAllBtn.interactable = false;
        });

        enhanceBtn.onClick.AddListener(() =>
        {
            if (selectedTab == EEquipmentType.Weapon)
            {
                var ret = EquipmentManager.instance.CanEnhance(selectedWeapon);
                if (ret == 1)
                    uiEnhancePopup.ShowUI(selectedWeapon);
                else if (ret == -1)
                    MessageUIManager.instance.ShowCenterMessage("최대 레벨입니다.");
            }
            else if (selectedTab == EEquipmentType.Armor)
            {
                var ret = EquipmentManager.instance.CanEnhance(selectedArmor); 
                if (ret == 1)
                    uiEnhancePopup.ShowUI(selectedArmor);
                else if (ret == -1)
                    MessageUIManager.instance.ShowCenterMessage("최대 레벨입니다.");
            }

            if (questGuide.gameObject.activeInHierarchy)
            {
                questGuide.gameObject.SetActive(false);
            }
        });

        uiEnhancePopup.actOnCallback += () => UpdateSelectedUI(selectedTab);
    }

    public void SelectEquipment(UIEquipment UIItem)
    {
        var item = UIItem.GetInfo();
        
        if (ReferenceEquals(item, null))
            return;

        StringBuilder sb = new StringBuilder();

        if (item.type == EEquipmentType.Weapon)
        {
            selectedWeapon = item as WeaponInfo;
            uiSelectedWeapon = UIItem;
        }
        else if (item.type == EEquipmentType.Armor)
        {
            selectedArmor = item as ArmorInfo;
            uiSelectedArmor = UIItem;
        }
        
        // 이미지
        if (EquipmentManager.instance.images.TryGetValue(item.equipName, out Sprite sprite))
            image.sprite = sprite;
        background.sprite = EquipmentManager.instance.GetFrame(item.rarity);
        // 색
        backEffecct.color = item.myColor;
        // 레벨
        level.text = "Lv." + item.enhancementLevel.ToString();
        // 개수
        count.text = sb.Clear().Append(item.Quantity).Append("/4").ToString();
        countSlider.value = item.Quantity;
        // 레어도
        rarity.text = sb.Clear().Append(Strings.rareKor[(int)item.rarity]).Append(" ").Append(item.level).ToString(); 
        // 보유 효과
        if (item.type == EEquipmentType.Weapon)
        {
            ownedEffectTitle.text = "공격력 증가";
            equipEffectTitle.text = "공격력 증가";
        }
        else if (item.type == EEquipmentType.Armor)
        {
            ownedEffectTitle.text = "체력 증가";
            equipEffectTitle.text = "체력 증가";
        }
        ownedEffect.text = CustomText.SetColor(sb.Clear().Append(" + ").Append(item.ownedEffect).ToString(), EColorType.Green);
        // 장착 효과
        // if (item.type == EEquipmentType.Weapon)
        //     equipEffectTitle.text = "공격력 증가";
        // else if (item.type == EEquipmentType.Armor)
        //     equipEffectTitle.text = "체력 증가";
        equipEffect.text = CustomText.SetColor(sb.Clear().Append(" + ").Append(item.equippedEffect).ToString(), EColorType.Green);
        // " + " + item.equippedEffect.ToString();
        // 각성 효과
        // if (item.type == EEquipmentType.Weapon)
        //     awakenEffectTitle.text = "보유 효과 증가";
        // else if (item.type == EEquipmentType.Armor)
        //     awakenEffectTitle.text = "보유 효과 증가";
        awakenEffectTitle.text = "보유 효과 증가";
        awakenEffect.text = CustomText.SetColor(sb.Clear().Append(" + ").Append(item.basicAwakenEffect).ToString(), EColorType.Green);// " + " + item.basicAwakenEffect.ToString();

        awakenEffectLock.SetActive(!item.isAwaken);

        compositeBtn.interactable = item.Quantity >= 4;
    }

    private void UpdateSelectedUI(EEquipmentType type)
    {
        switch (type)
        {
            case EEquipmentType.Weapon:
                UpdateSelectedUI(selectedWeapon);
                uiSelectedWeapon.UpdateEnhanceLevelUI();
                autoEquipBtn.interactable = EquipmentManager.instance.TryGetBestItem(type).equipName != (PlayerManager.instance.EquippedWeapon?.equipName ?? "");
                break;
            case EEquipmentType.Armor:
                UpdateSelectedUI(selectedArmor);
                uiSelectedArmor.UpdateEnhanceLevelUI();
                autoEquipBtn.interactable = EquipmentManager.instance.TryGetBestItem(type).equipName != (PlayerManager.instance.EquippedArmor?.equipName ?? "");
                break;
        }
    }

    private void UpdateSelectedUI(Equipment item)
    {
        if (ReferenceEquals(item, null)) return;
        
        StringBuilder sb = new StringBuilder();
        
        count.text = sb.Clear().Append(item.Quantity).Append("/4").ToString();// item.Quantity.ToString() + "/4";
        countSlider.value = item.Quantity;
        ownedEffect.text = CustomText.SetColor(sb.Clear().Append(" + ").Append(item.ownedEffect).ToString(), EColorType.Green);//" + " + item.ownedEffect.ToString();
        equipEffect.text = CustomText.SetColor(sb.Clear().Append(" + ").Append(item.equippedEffect).ToString(), EColorType.Green);//" + " + item.equippedEffect.ToString();

        compositeBtn.interactable = item.Quantity >= 4;
    }

    public override void ShowQuestRoot(EAchievementType type)
    {
        switch (type)
        {
            case EAchievementType.WeaponEquip:
                questGuide.position = equipQuestRoot.position;
                break;
            case EAchievementType.ArmorEquip:
                questGuide.position = equipQuestRoot.position;
                break;
            case EAchievementType.WeaponCompositeCount:
                questGuide.position = compositeQuestRoot.position;
                break;
            case EAchievementType.ArmorCompositeCount:
                questGuide.position = compositeQuestRoot.position;
                break;
            case EAchievementType.EquipEnhanceCount:
                questGuide.position = enhanceQuestRoot.position;
                break;
        }
        questGuide.gameObject.SetActive(true);
        QuestManager.instance.currentQuest.onComplete += (x) => questGuide.gameObject.SetActive(false);
        uiEnhancePopup.ShowQuestRoot(type);
    }
}