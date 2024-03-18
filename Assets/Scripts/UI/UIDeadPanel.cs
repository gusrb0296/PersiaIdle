using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDeadPanel : UIPanel
{
    [SerializeField] private float duration;

    private float elaspedTime;

    private void Update()
    {
        elaspedTime += Time.deltaTime;
        if (elaspedTime > duration)
        {
            CloseUI(); 
        }
    }

    public override void ShowUI()
    {
        elaspedTime = .0f;
        base.ShowUI();
    }
}
