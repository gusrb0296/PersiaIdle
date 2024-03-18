using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public RectTransform top;
    public RectTransform bottom;
    public RectTransform panel;
    public RectTransform front;
    public RectTransform message;
    [Header("확인용")]
    public List<UIBase> uiBases;

    private Dictionary<string, List<UIBase>> uis;

    private void Awake()
    {
        instance = this;
        uis = new Dictionary<string, List<UIBase>>();
        SortUIToDic();
    }

    public void InitUIManager()
    {
        foreach (var ui in uiBases)
        {
            ui.InitUI(null);
        }
    }

    private void SortUIToDic()
    {
        foreach (var ui in uiBases)
        {
            uis.TryAdd(ui.GetType().ToString(), new List<UIBase>());
            uis[ui.GetType().ToString()].Add(ui);
        }
    }

    public void AddUIToList(UIBase[] _uiBases)
    {
        this.uiBases.AddRange(_uiBases);
    }

    public T TryGetUI<T>(int index = 0) where T : UIBase
    {
        if (uis.TryGetValue(typeof(T).ToString(), out List<UIBase> value))
            return value[index] as T;
        else
            return null;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(UIManager))]
public class UIManagerCustomEditor : Editor
{
    private UIManager uiManager;

    private void OnEnable()
    {
        uiManager = target as UIManager;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("===== Find All UIBase ====="))
        {
            FindAllUIBaseInScene();
            EditorUtility.SetDirty(target);
        }
        base.OnInspectorGUI();
    }

    private void FindAllUIBaseInScene()
    {
        uiManager.uiBases.Clear();

        List<UIBase> allUI = GetAllUIBase(uiManager.gameObject);
        uiManager.AddUIToList(allUI.ToArray());
    }

    private List<UIBase> GetAllUIBase(GameObject obj)
    { 
        var limit = obj.transform.childCount;

        List<UIBase> allUI = new List<UIBase>();
        
        for (int i = 0; i < limit; ++i)
        {
            var child = obj.transform.GetChild(i);
            allUI.AddRange(GetAllUIBase(child.gameObject));
        }

        if (obj.TryGetComponent<UIBase>(out UIBase ui))
            allUI.Add(ui);

        return allUI;
    }
}
#endif