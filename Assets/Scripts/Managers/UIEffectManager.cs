using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class UIEffectManager : MonoBehaviour
{
    public static UIEffectManager instance;

    [Header("Click Effect")]
    [SerializeField] private UIEffect clickEffect;
    [SerializeField] private RectTransform clickRoot;
    [SerializeField] private int clickPoolSize;
    private CustomPool<UIEffect> clickPool;

    [Header("Upgrade Effect")]
    [SerializeField] private UIEffect upgradeEffect;
    [SerializeField] private RectTransform upgradeRoot;
    [SerializeField] private int upgradePoolSize;
    private CustomPool<UIEffect> upgradePool; 

    private void Awake()
    {
        instance = this;
    }

    public void InitEffectUIManager()
    {
        clickPool = EasyUIPooling.MakePool(clickEffect, clickRoot, (ui)=>ui.actOnCallback += () => clickPool.Release(ui), null, null, clickPoolSize, true);
        upgradePool = EasyUIPooling.MakePool(upgradeEffect, upgradeRoot, (ui)=> ui.actOnCallback += () => upgradePool.Release(ui), null, null, upgradePoolSize, true);
    }

    // public void InitRoot(RectTransform clickRoot, RectTransform upgradeRoot)
    // {
    //     this.clickRoot = clickRoot;
    //     this.upgradeRoot = upgradeRoot;
    // }

    public void ShowClickEffect(Vector3 screenPosition)
    {
        var effect = clickPool.Get();
        effect.Self.position = screenPosition;
    }

    public void ShowUpgradeEffect(Transform target)
    {
        var effect = upgradePool.Get();
        effect.transform.position = target.transform.position;
    }
}
