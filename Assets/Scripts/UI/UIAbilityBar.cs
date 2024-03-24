using Defines;
using TMPro;
using UnityEngine;
using Utils;

public class UIAbilityBar : UIBase
{
    [SerializeField] private HoldCheckerButton upgradeBtn;

    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text abilityGradeText;
    [SerializeField] private TMP_Text abilityNameText;

    private AbilityUpgradeInfo upgradeInfo;

    public void ShowUI(AbilityUpgradeInfo info)
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
            // 등급 단계별로 색깔 변경
            if (upgradeInfo.abilityType == EAbilityType.C)
            {
                abilityGradeText.color = Color.white;
            }
            else if(upgradeInfo.abilityType == EAbilityType.B)
            {
                abilityGradeText.color = Color.green;
            }
            else if (upgradeInfo.abilityType == EAbilityType.A)
            {
                abilityGradeText.color = Color.blue;
            }
            else if (upgradeInfo.abilityType == EAbilityType.S)
            {
                abilityGradeText.color = Color.magenta;
            }
            else
            {
                abilityGradeText.color = Color.red;
            }

        }
    }

    private void Awake()
    {
        InitializeBtn();
    }

    private void InitializeBtn()
    {
        upgradeBtn.onClick.AddListener(() => UpgradeBtn(upgradeInfo.abilityType));
        upgradeBtn.onExit.AddListener(CurrencyManager.instance.SaveCurrencies);
    }

    private void UpgradeBtn(EAbilityType type)
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

    private bool TryUpgrade(EAbilityType type)
    {
        if (CurrencyManager.instance.SubtractCurrency(upgradeInfo.currencyType, upgradeInfo.cost))
        {
            //if (upgradeInfo.upgradePerLevelInt != 0)
            //    UpgradeManager.instance.UpgradeBaseStatus(upgradeInfo);
            //else
            //    UpgradeManager.instance.UpgradeBaseStatus(upgradeInfo);

            //// Show Effect
            //UIEffectManager.instance.ShowUpgradeEffect(image.transform);

            return true;
        }
        else
        {
            return false;
        }
    }

    private void UpdateUI()
    {
    }

    private void InitializeUI()
    {

    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }
}
