using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UIBase : MonoBehaviour
{
    public event Action actOnShow;
    public event Action actOnDisable;
    public event Action actOnDestory;
    public event Action actOnCallback;
    public RectTransform Self
    {
        get
        {
            if (self == null)
                self = gameObject.GetComponent<RectTransform>();
            return self;
        }
    }

    protected RectTransform self;
    protected UIBase parent;
    protected List<UIBase> child;
    [SerializeField] protected TMP_Text[] textTitles;
    [SerializeField] protected TMP_Text[] textDatas;
    
    public virtual void ResizeUI(float scale)
    {
        Self.localScale = scale * Vector3.one;
    }

    public virtual void ResizeUI(float scale, float titleFontSize, float dataFontSize)
    {
        Self.localScale = scale * Vector3.one;
        foreach (var text in textTitles)
        {
            text.fontSize = titleFontSize;
        }
        foreach (var text in textDatas)
        {
            text.fontSize = dataFontSize;
        }
    }
    
    public virtual void ShowUI()
    {
        gameObject.SetActive(true);
        actOnShow?.Invoke();
    }

    public virtual void CloseUI(float afterseconds)
    {
        StartCoroutine(Closing(afterseconds));
    }

    protected IEnumerator Closing(float time)
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        CloseUI();
    }

    public virtual void CloseUI()
    {
        actOnCallback?.Invoke();
    }
    
    public virtual UIBase InitUI(UIBase parent)
    {
        this.parent = parent;
        return this;
    }

    protected virtual void OnDisable()
    {
        actOnDisable?.Invoke();
    }

    protected virtual void OnDestroy()
    {
        actOnDestory?.Invoke();
    }

    public virtual void ShowQuestRoot(EAchievementType type)
    {
        
    }

}
