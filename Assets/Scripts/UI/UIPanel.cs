using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanel : UIBase
{
    [SerializeField] protected Button[] exitBtns;

    public override UIBase InitUI(UIBase parent)
    {
        this.parent = parent;
        InitializeBtns();
        return this;
    }

    private void OnEnable()
    {
        foreach(var btn in exitBtns)
            btn.gameObject.SetActive(true);
    }

    public override void CloseUI()
    {
        base.CloseUI();
        gameObject.SetActive(false);
        
        foreach(var btn in exitBtns)
            btn.gameObject.SetActive(false);
    }
    
    protected virtual void InitializeBtns()
    {
        foreach (var btn in exitBtns)
        {
            btn.onClick.AddListener(CloseUI);
        }
    }
}
