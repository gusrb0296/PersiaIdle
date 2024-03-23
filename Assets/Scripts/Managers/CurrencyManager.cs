using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Diagnostics.Tracing;
using Keiwando.BigInteger;
using UnityEngine.UI;


public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance;

    // 이벤트 : 통화의 양이 변경될 때 발생
    public event Action<ECurrencyType, string> onCurrencyChanged;

    // 모든 통화의 목록 
    public List<Currency> currencies;
    public Dictionary<ECurrencyType, Currency> currencyDictionary;

    public List<Sprite> currencyIcons;
    public Dictionary<ECurrencyType, Sprite> currencyIcon;

    private WaitForSeconds wait;
    private void Awake()
    {
        instance = this;
        currencyIcon = new Dictionary<ECurrencyType, Sprite>();
        currencyDictionary = new Dictionary<ECurrencyType, Currency>();

        for (int i = 0; i < currencyIcons.Count; ++i)
        {
            currencyIcon.Add((ECurrencyType)i, currencyIcons[i]);
        }

        wait = new WaitForSeconds(20.0f);
        StartCoroutine(AutoSave());
    }

    IEnumerator AutoSave()
    {
        while (true)
        {
            yield return wait;
            SaveCurrencies();
            PlayerManager.instance.levelSystem.SaveLevelExp(PlayerManager.instance.userName);
        }
    }

    // 재화 매니저 초기화 메서드
    public void InitCurrencyManager()
    {
        if (!LoadCurrencies())
        {
            // currencies = new List<Currency>();
            SaveCurrencies();
            LoadCurrencies();
        }

        foreach (var currency in currencies)
        {
            if(!currencyDictionary.ContainsKey(currency.type))
                currencyDictionary.Add(currency.type, currency);
        }
        
        UIManager.instance.TryGetUI<UICurrencyUpdater>().ShowUI();
    }

    public Sprite GetIcon(ECurrencyType type)
    {
        return currencyIcon[type];
    }

    public Sprite GetIconOrNull(EQuestRewardType type)
    {
        if ((int)type <= (int)ECurrencyType.EnhanceInvitation)
            return currencyIcon[(ECurrencyType)type];
        return null;
    }

    // 특정 통화를 증가시키는 메서드
    public void AddCurrency(ECurrencyType type, BigInteger value)
    {
        Currency currency = currencies.Find(c => c.currencyName == type.ToString());
        if (currency != null)
        {
            currency.Add(value);
            onCurrencyChanged?.Invoke(type, currency.amount); // 이벤트 발생
            if (type == ECurrencyType.Dia)
                SaveCurrencies();
            // SaveCurrency(type);
        }
    }

    // 특정 통화를 감소시키는 메서드
    public bool SubtractCurrency(ECurrencyType type, BigInteger value)
    {
        // 모든 통화중 매개변수로 받은 이름이 있나 체크
        Currency currency = currencies.Find(c => c.type == type);
        if (currency != null)
        {
            // 통화의 양을 감소시키, 결과에 따라 이벤트 발생
            bool result = currency.Subtract(value);
            // SaveCurrencies();
            if (result)
            {
                onCurrencyChanged?.Invoke(currency.type, currency.amount);
            }
            return result;
        }
        return false;
    }

    // 특정 통화의 현재 양을 반환하는 메서드
    public string GetCurrencyStr(ECurrencyType type)
    {
        Currency currency = currencies.Find(c => c.type == type);
        return currency?.amount ?? "0";
        // BigInteger ret = new BigInteger(currencyDictionary[type].value.ToString());
        // return ret;
    }

    // 모든 통화를 로컬에 저장시키는 메서드
    public void SaveCurrencies()
    {
        DataManager.Instance.Save<List<Currency>>("currencies", currencies);
    }

    // public void SaveCurrency(ECurrencyType type)
    // {
    //     ES3.Save<Currency>("currency", currencies.Find(c => c.type == type));
    // }

    // 로컬에 저장되어있는 모든 통화를 불러오는 메서드
    public bool LoadCurrencies()
    {
        if (ES3.KeyExists("currencies"))
        {
            currencies = DataManager.Instance.Load<List<Currency>>("currencies");
            foreach (Currency currency in currencies)
            {
                onCurrencyChanged?.Invoke(currency.type, currency.amount); // 로딩 후 이벤트 발생
            }
        }
        else return false;
        return true;
    }

    // public BigInteger GetCurrency(ECurrencyType currencyType)
    // {
    //     return currencyDictionary[currencyType].value;
    // }
    public BigInteger GetCurrency(ECurrencyType upgradeInfoCurrencyType)
    {
        if (currencyDictionary.ContainsKey(upgradeInfoCurrencyType))
            return new BigInteger(currencyDictionary[upgradeInfoCurrencyType].amount);
        else
            return new BigInteger(0);
        //throw new NotImplementedException();
        
        
    }
}
