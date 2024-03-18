using System;
using Defines;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UIStatusBar : UIBase
{
    [SerializeField] private HoldCheckerButton upgradeBtn;
    
    [SerializeField] private Image image;
    
    [SerializeField] private Image costImage;
    
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text maxLevelText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text upgradePerLevelText;
    [SerializeField] private TMP_Text totalUpgrade;
    
    private StatUpgradeInfo upgradeInfo;

    [SerializeField] private RectTransform effectRect;

    public Transform GetButtonRect()
    {
        return upgradeBtn.transform;
    }
    public void ShowUI(StatUpgradeInfo info)
    {
        upgradeInfo = info;
        InitializeUI();

        CurrencyManager.instance.onCurrencyChanged += OnCurrencyUpdate;
    }

    public override void CloseUI()
    {
        base.CloseUI();

        CurrencyManager.instance.onCurrencyChanged -= OnCurrencyUpdate;
    }

    private void OnCurrencyUpdate(ECurrencyType type, string amount)
    {
        if (type == upgradeInfo.currencyType)
        {
            if (upgradeInfo.CheckUpgradeCondition())
            {
                // TODO 글씨 색 회색
                upgradeBtn.interactable = true;
                costText.color = Color.white;
            }
            else
            {
                // TODO 글씨 색 흰색 
                upgradeBtn.interactable = false;
                costText.color = Color.red;
            }
        }
    }

    protected void Awake()
    {
        InitializeBtn();
    }

    private void InitializeBtn()
    {
        upgradeBtn.onClick.AddListener(() => UpgradeBtn(upgradeInfo.statusType));
        upgradeBtn.onExit.AddListener(CurrencyManager.instance.SaveCurrencies);
    }

    private void UpgradeBtn(EStatusType type)
    {
        // TODO currency manager를 통해서 돈 빼기!
        if (TryUpgrade(type))
        {
            UpdateUI();
        }
        else
        {
            MessageUIManager.instance.ShowCenterMessage(CustomText.SetColor("골드", Color.yellow) + "가 부족합니다.");
        }
    }

    private bool TryUpgrade(EStatusType type)
    {
        if (CurrencyManager.instance.SubtractCurrency(upgradeInfo.currencyType, upgradeInfo.cost))
        {
            if (upgradeInfo.upgradePerLevelInt != 0)
                UpgradeManager.instance.UpgradeBaseStatus(upgradeInfo);
            else
                UpgradeManager.instance.UpgradeBaseStatus(upgradeInfo);
            
            // Show Effect
            UIEffectManager.instance.ShowUpgradeEffect(image.transform);

            return true;
        }
        else
        {
            return false;
        }
    }

    private void UpdateUI()
    {
        levelText.text = upgradeInfo.level.ToString();
        
        if (upgradeInfo.upgradePerLevelInt != 0)
            totalUpgrade.text = $"(+{upgradeInfo.upgradePerLevelInt * upgradeInfo.level})";
        else
            totalUpgrade.text = $"(+{(upgradeInfo.upgradePerLevelFloat * upgradeInfo.level * 100):F2}%)";

        costText.text = upgradeInfo.cost.ChangeToShort();
        
        upgradeBtn.interactable = upgradeInfo.CheckUpgradeCondition();
    }

    private void InitializeUI()
    {
        if (!ReferenceEquals(upgradeInfo.image,null))
            image.sprite = upgradeInfo.image;

        costImage.sprite = CurrencyManager.instance.GetIcon(upgradeInfo.currencyType);
       
        if (upgradeInfo.upgradePerLevelInt != 0)
            upgradePerLevelText.text = upgradeInfo.upgradePerLevelInt.ToString();
        else
            upgradePerLevelText.text = (upgradeInfo.upgradePerLevelFloat * 100).ToString("F2") + "%";

        titleText.text = upgradeInfo.title + " <color=#16FF00>+</color>";
        maxLevelText.text = upgradeInfo.maxLevel.ToString();
        
        UpdateUI();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }
}
