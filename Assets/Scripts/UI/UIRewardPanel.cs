using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UIRewardPanel : UIPanel
{
    // public Image[] currencyIcon;
    // public TMP_Text[] currencyAmount;
    private CustomPool<UIRewardElement> rewardPool;
    [SerializeField] UIRewardElement prefab;
    [SerializeField]private RectTransform root;
    [SerializeField]private int poolSize;

    public override UIBase InitUI(UIBase parent)
    {
        base.InitUI(parent);
        rewardPool = EasyUIPooling.MakePool(prefab, root, ui => ui.actOnCallback += () => rewardPool.Release(ui), null,
            null, poolSize, true);
        return this;
    }

    public void ShowUI(ECurrencyType type, BigInteger amount)
    {
        base.ShowUI();
        var ui = rewardPool.Get();
        ui.ShowUI(CurrencyManager.instance.GetIcon(type), amount.ChangeToShort());
    }

    public void ShowUI(ECurrencyType[] types, BigInteger[] amount)
    {
        for (int i = 0; i < types.Length; ++i)
        {
            var ui = rewardPool.Get();
            ui.ShowUI(CurrencyManager.instance.GetIcon(types[i]), amount[i].ChangeToShort());
        }

        // for (int i = 0; i < types.Length; ++i)
        // {
        //     currencyIcon[i].sprite = CurrencyManager.instance.GetIcon(types[i]);
        //     currencyAmount[i].text = amount[i].ChangeToShort();
        // }
    }

    public override void CloseUI()
    {
        base.CloseUI();

        while (rewardPool.UsedCount > 0)
        {
            rewardPool.UsedList.First.Value.CloseUI();
        }
    }
}