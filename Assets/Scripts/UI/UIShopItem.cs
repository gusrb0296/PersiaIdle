using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIShopItem : UIBase
{
    [SerializeField] private Image backEffect;
    [SerializeField] private Image itemImage;
    [SerializeField] private Image frontEffect;
    [SerializeField] private GameObject bestTag;
    
    [SerializeField] private TMP_Text rewardTitle;
    [SerializeField] private GameObject bonusTag;
    [SerializeField] private TMP_Text bonusPercentage;
    [SerializeField] private GameObject bonusPlus;
    [SerializeField] private GameObject bonusValueObj;
    [SerializeField] private TMP_Text bonusValue;
    
    [SerializeField] private Image priceImage;
    [SerializeField] private TMP_Text price;

    [SerializeField] private Button btn;
    
    private ShopItemInfo itemInfo;

    public virtual void ShowUI(ShopItemInfo item)
    {
        base.ShowUI();
        itemInfo = item;
        
        backEffect.sprite = item.backEffect;
        backEffect.color = item.backColor;
        itemImage.sprite = item.itemImage;
        frontEffect.sprite = item.frontEffect;
        if (item.isBest)
            bestTag.SetActive(true);
        else
            bestTag.SetActive(false);

        rewardTitle.text = item.rewardStr;
        if (item.isBonus)
        {
            bonusTag.SetActive(true);
            bonusPercentage.text = item.bonusStr;
            bonusValue.text = item.bonusValue;
            bonusPlus.SetActive(true);
            bonusValueObj.SetActive(true);
        }
        else
        {
            bonusTag.SetActive(false);
            bonusPlus.SetActive(false);
            bonusValueObj.SetActive(false);
        }
        
        priceImage.sprite = item.priceImage;
        price.text = item.priceStr;
    }

    protected void Awake()
    {
        InitializeBtn();
    }

    private void InitializeBtn()
    {
        btn.onClick.AddListener(ShowPopup);
    }

    private void ShowPopup()
    {
        Debug.Log("TODO Popup UI!");
    }
}
