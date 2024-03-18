using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRewardElement : UIBase
{
    public Image currencyIcon;
    public Image frame;
    public TMP_Text currencyAmount;

    public void ShowUI(Sprite icon, string amount)
    {
        base.ShowUI();
        currencyIcon.sprite = icon;
        currencyAmount.text = amount;
    }

    public void ShowUI(Reward reward)
    {
        if (reward.type < ENormalRewardType.Weapon)
        {
            currencyIcon.sprite = CurrencyManager.instance.GetIcon((ECurrencyType)reward.type);
            currencyAmount.text = reward.amount.ChangeToShort();
        }
        else
        {
            if (reward.type == ENormalRewardType.Weapon)
                currencyIcon.sprite = EquipmentManager.instance.GetIcon(EEquipmentType.Weapon, BigInteger.ToInt32(reward.amount));
            else
                currencyIcon.sprite = EquipmentManager.instance.GetIcon(EEquipmentType.Armor, BigInteger.ToInt32(reward.amount));
            frame.sprite = EquipmentManager.instance.GetFrame((ERarity)(BigInteger.ToInt32(reward.amount)/4));

            currencyAmount.text = $"{Strings.rareKor[(BigInteger.ToInt32(reward.amount)/4)]} {BigInteger.ToInt32(reward.amount)%4+1}";
        }
    }
}
