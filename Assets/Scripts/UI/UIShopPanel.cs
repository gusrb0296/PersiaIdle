using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using UI;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR.Provider;
using Utils;

public class UIShopPanel : UIPanel
{
    [SerializeField] private Button[] topBtns;

    [SerializeField] private RectTransform root;
    [SerializeField] private UIShopItem shopItemPrefab;
    [SerializeField] private int shopItemPoolSize;
    private CustomPool<UIShopItem> pool;

    [SerializeField] private ShopItemInfo[] testGoldItems;
    [SerializeField] private ShopItemInfo[] testGemItems;
    [SerializeField] private ShopItemInfo[] testPackageItems;
    [SerializeField] private ShopItemInfo[] testDarkMarketItems;

    protected void Awake()
    {
        pool = EasyUIPooling.MakePool(shopItemPrefab, root, null, x=> x.transform.SetAsLastSibling(),
            null, shopItemPoolSize, true);
    }

    private void OnEnable()
    {
        OpenShop(EShopType.Gold);
    }

    public virtual void ShowUI(ShopItemInfo[] gold = null, ShopItemInfo[] gem = null, ShopItemInfo[] package = null,
        ShopItemInfo[] darkMarket = null)
    {
        if (!ReferenceEquals(gold, null))
            testGoldItems = gold;
        if (!ReferenceEquals(gem, null))
            testGemItems = gem;
        if (!ReferenceEquals(package, null))
            testPackageItems = package;
        if (!ReferenceEquals(darkMarket, null))
            testDarkMarketItems = darkMarket;
    }

    public void EnableBtnExceptThis(int index)
    {
        for (int i = 0; i < topBtns.Length; ++i)
        {
            topBtns[i].interactable = (i != index);
        }
    }

    private void ClearShop()
    {
        pool.Clear();
    }

    private void OpenShop(EShopType type)
    {
        ClearShop();
        EnableBtnExceptThis((int)type);
        switch (type)
        {
            case EShopType.Gold:
                foreach (var item in testGoldItems)
                {
                    var obj = pool.Get();
                    obj.ShowUI(item);
                }
                
                break;
            case EShopType.Dia:
                foreach (var item in testGemItems)
                {
                    var obj = pool.Get();
                    obj.ShowUI(item);
                }

                break;
            case EShopType.Package:
                foreach (var item in testPackageItems)
                {
                    var obj = pool.Get();
                    obj.ShowUI(item);
                }

                break;
            case EShopType.DarkMarket:
                foreach (var item in testDarkMarketItems)
                {
                    var obj = pool.Get();
                    obj.ShowUI(item);
                }

                break;
        }
    }

    protected override void InitializeBtns()
    {
        for (int i = 0; i < topBtns.Length; ++i)
        {
            EShopType type = (EShopType)i;
            topBtns[i].onClick.AddListener(() => OpenShop(type));
        }
        base.InitializeBtns();
    }
}