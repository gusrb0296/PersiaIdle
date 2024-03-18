using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICurrencyUpdater : UIBase
{
    public UICurrency[] currencies;
    
    private Dictionary<ECurrencyType, TMP_Text> currencyUI;
    public override void ShowUI()
    {
        base.ShowUI();
        
        currencyUI = new Dictionary<ECurrencyType, TMP_Text>();
        foreach (var currency in currencies)
        {
            currencyUI.Add(currency.currencyType, currency.currencyText);
            currency.currencyIcon.sprite = CurrencyManager.instance.GetIcon(currency.currencyType);
            UpdateCurrencyUI(currency.currencyType, CurrencyManager.instance.GetCurrencyStr(currency.currencyType));
        }

        CurrencyManager.instance.onCurrencyChanged += UpdateCurrencyUI;
    }

    public override void CloseUI()
    {
        base.CloseUI();
        
        CurrencyManager.instance.onCurrencyChanged -= UpdateCurrencyUI;
    }

    // 통화의 UI를 업데이트 시키는 메서드
    void UpdateCurrencyUI(ECurrencyType type, string amount)
    {
        if (currencyUI.TryGetValue(type, out var ui))
        {
            switch (type)
            {
                case ECurrencyType.Gold:
                case ECurrencyType.EnhanceStone:
                case ECurrencyType.AwakenStone:
                case ECurrencyType.WeaponSummonTicket:
                case ECurrencyType.ArmorSummonTicket:
                case ECurrencyType.Exp:
                    ui.text = BigInteger.ChangeToShort(amount);
                    break;
                case ECurrencyType.Dia:
                case ECurrencyType.GoldInvitation:
                case ECurrencyType.AwakenInvitation:
                case ECurrencyType.EnhanceInvitation:
                    ui.text = amount;
                    break;
            }
        }
    }

    public void ShowCurrency(ECurrencyType type, bool onoff)
    {
        foreach (var currency in currencies)
        {
            if (currency.currencyType == type)
                currency.currency.SetActive(onoff);
        }
    }
}

[Serializable]
public class UICurrency
{
    public GameObject currency;
    public Image currencyIcon;
    public TMP_Text currencyText;
    public ECurrencyType currencyType;
}