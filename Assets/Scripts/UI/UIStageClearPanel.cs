using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Utils;

public class UIStageClearPanel : UIPanel
{
    [SerializeField] private float duration;
    private float elaspedTime;
    [SerializeField] private UIRewardElement elementPrefab;
    [SerializeField] private RectTransform elementRoot;
    [SerializeField] private int elementPoolSize;
    [SerializeField] private bool elementPoolResizable;
    private CustomPool<UIRewardElement> uiStageClearElementPool;

    public override UIBase InitUI(UIBase _parent)
    {
        base.InitUI(_parent);
        uiStageClearElementPool = EasyUIPooling.MakePool(elementPrefab, elementRoot,
            (ui) => ui.actOnCallback += () => uiStageClearElementPool.Release(ui),
            ui => ui.transform.SetAsLastSibling(), null, elementPoolSize, elementPoolResizable);
        return this;
    }

    private void Update()
    {
        elaspedTime += Time.deltaTime;
        if (elaspedTime > duration)
        {
            CloseUI();
        }
    }

    public void ShowUI(MonsterDropData[] rewardDatas)
    {
        CloseUI();
        
        base.ShowUI();
        elaspedTime = .0f;

        for (int i = 0; i < rewardDatas.Length; ++i)
        {
            var ui = uiStageClearElementPool.Get();
            switch ((ECurrencyType)rewardDatas[i].rewardType)
            {
                case ECurrencyType.Gold:
                case ECurrencyType.EnhanceStone:
                case ECurrencyType.AwakenStone:
                case ECurrencyType.WeaponSummonTicket:
                case ECurrencyType.ArmorSummonTicket:
                case ECurrencyType.Exp:
                    ui.ShowUI(CurrencyManager.instance.GetIcon((ECurrencyType)rewardDatas[i].rewardType), rewardDatas[i].straightRewardAmount.ChangeToShort());
                    break;
                case ECurrencyType.Dia:
                case ECurrencyType.GoldInvitation:
                case ECurrencyType.AwakenInvitation:
                case ECurrencyType.EnhanceInvitation:
                    ui.ShowUI(CurrencyManager.instance.GetIcon((ECurrencyType)rewardDatas[i].rewardType), rewardDatas[i].straightRewardAmount.ToString());
                    break;
            }
        }
    }

    public override void CloseUI()
    {
        base.CloseUI();
        
        uiStageClearElementPool.Clear();
    }
}