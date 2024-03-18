using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIConfirm : UIBase
{
    [SerializeField] private Button okBtn;
    [SerializeField] private Button cancelBtn;
    [SerializeField] private TMP_Text descriptionText;

    public void ShowUI(string description, Action onOkBtn, Action onCancel = null)
    {
        base.ShowUI();
        descriptionText.text = description;
        
        okBtn.onClick.AddListener(onOkBtn.Invoke);
        okBtn.onClick.AddListener(CloseUI);
        
        if (!ReferenceEquals(onCancel, null))
            cancelBtn.onClick.AddListener(onCancel.Invoke);
        cancelBtn.onClick.AddListener(CloseUI);
    }

    public void AddOnOK(Action onOkBtn)
    {
        okBtn.onClick.AddListener(onOkBtn.Invoke);
    }

    public void AddOnCancel(Action onCancelBtn)
    {
        cancelBtn.onClick.AddListener(onCancelBtn.Invoke);
    }

    public override void CloseUI()
    {
        base.CloseUI();
        okBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.RemoveAllListeners();
        gameObject.SetActive(false);
    }
}