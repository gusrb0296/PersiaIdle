using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

public class UIAchievement : UIPanel
{
    [Header("업적 세부 UI prefab")]
    [SerializeField] private UIAchievementElement element;
    [Header("세부 UI 생성 Hierarchy 부모")]
    [SerializeField] private RectTransform rootUI;

    // 세부 UI를 관리하기 위한 변수
    #region Field for Runtime
    private CustomPool<UIAchievementElement> elementUIPool;
    #endregion
    public override UIBase InitUI(UIBase _parent)
    {
        base.InitUI(_parent);
        elementUIPool = EasyUIPooling.MakePool(element, rootUI,
            (ui) => { ui.actOnCallback += () => elementUIPool.Release(ui); }, null, null, 5, true);
        return this;
    }

    // 업적 UI를 활성화시키는 메소드
    public override void ShowUI()
    {
        base.ShowUI();
        
        foreach (var achieve in QuestManager.instance.ProgressQuest)
        {
            var obj = elementUIPool.Get();
            obj.ShowUI(achieve);
        }
    }

    public override void CloseUI()
    {
        base.CloseUI();
        
        elementUIPool.Clear();
    }
}