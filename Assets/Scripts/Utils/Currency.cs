using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine.UI;

[Serializable]
public class Currency
{
    public string currencyName
    {
        get => type.ToString();
    }

    public ECurrencyType type;
    public string amount;

    public void Add(BigInteger value)
    {
        BigInteger currentAmount = new BigInteger(amount);
        currentAmount += value;
        amount = currentAmount.ToString();
    }

    public bool Subtract(BigInteger value)
    {
        BigInteger currentAmount = new BigInteger(amount);
        if (currentAmount - value < 0) return false;
        currentAmount -= value;
        amount = currentAmount.ToString();
        return true;
    }
    
    public Currency(ECurrencyType type, string initialAmount)
    {
        this.type = type;
        this.amount = initialAmount;
    }
}