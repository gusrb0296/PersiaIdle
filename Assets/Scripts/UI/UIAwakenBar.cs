using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using Defines;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UIAwakenBar : UIBase
{
    [SerializeField] private HoldCheckerButton upgradeBtn;
    
    [SerializeField] private Image image;
    
    [SerializeField] private Image costImage;
    
    [SerializeField] private TMP_Text gemText;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text maxLevelText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text upgradePerLevelText;
    [SerializeField] private TMP_Text totalUpgrade;
    
    private AwakenUpgradeInfo upgradeInfo;
    public void ShowUI(AwakenUpgradeInfo info)
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
                // upgradeBtn.interactable = false;
                costText.color = Color.white;
            }
            else
            {
                // TODO 글씨 색 흰색 
                // upgradeBtn.interactable = true;
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
            MessageUIManager.instance.ShowCenterMessage(CustomText.SetColor("각성석", Color.blue)+"이 부족합니다.");
        }
    }

    private bool TryUpgrade(EStatusType type)
    {
        if (CurrencyManager.instance.SubtractCurrency(upgradeInfo.currencyType, upgradeInfo.cost))
        {
            if (upgradeInfo.upgradePerLevelInt != 0)
                UpgradeManager.instance.UpgradePercentStatus(upgradeInfo);
            else
                UpgradeManager.instance.UpgradePercentStatus(upgradeInfo);
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
            totalUpgrade.text = $"(+{upgradeInfo.upgradePerLevelInt * upgradeInfo.level})%";
        else
            totalUpgrade.text = $"(+{(upgradeInfo.upgradePerLevelFloat * upgradeInfo.level * 100):N0}%)";

        costText.text = upgradeInfo.cost.ChangeToShort();
        //
        // if (upgradeInfo.CheckUpgradeCondition())
        //     upgradeBtn.interactable = false;
        // else
        //     upgradeBtn.interactable = true;
    }

    private void InitializeUI()
    {
        if (!ReferenceEquals(upgradeInfo.image,null))
            image.sprite = upgradeInfo.image;

        costImage.sprite = CurrencyManager.instance.GetIcon(upgradeInfo.currencyType);
       
        if (upgradeInfo.upgradePerLevelInt != 0)
            upgradePerLevelText.text = upgradeInfo.upgradePerLevelInt.ToString() + "%";
        else
            upgradePerLevelText.text = (upgradeInfo.upgradePerLevelFloat * 100).ToString("F2") + "%";

        gemText.text = upgradeInfo.gemName;
        titleText.text = upgradeInfo.title + " <color=#16FF00>+</color>";
        maxLevelText.text = upgradeInfo.maxLevel.ToString();
        
        UpdateUI();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public Transform GetButtonRect()
    {
        return upgradeBtn.transform;
    }
}
