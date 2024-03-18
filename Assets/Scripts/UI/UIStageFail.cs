 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIStageFail : UIBase
{
    [SerializeField] private Button summonPanelBtn;
    [SerializeField] private Button trainingPanelBtn;
    [SerializeField] private Button equipmentPanelBtn;
    [SerializeField] private Button skillPanelBtn;
    [SerializeField] private Button exitBtn;

    public override UIBase InitUI(UIBase parent)
    {
        this.parent = parent;
        InitializeBtns();
        return this;
    }


    protected virtual void InitializeBtns()
    {
        summonPanelBtn.onClick.AddListener(UIManager.instance.TryGetUI<UISummonPanel>().ShowUI);
        trainingPanelBtn.onClick.AddListener(UIManager.instance.TryGetUI<UIGrowthPanel>().ShowUI);
        equipmentPanelBtn.onClick.AddListener(UIManager.instance.TryGetUI<UIEquipmentPanel>().ShowUI);
        skillPanelBtn.onClick.AddListener(UIManager.instance.TryGetUI<UISkillPanel>().ShowUI);
        
        summonPanelBtn.onClick.AddListener(()=>gameObject.SetActive(false));
        trainingPanelBtn.onClick.AddListener(()=>gameObject.SetActive(false));
        equipmentPanelBtn.onClick.AddListener(()=>gameObject.SetActive(false));
        skillPanelBtn.onClick.AddListener(()=>gameObject.SetActive(false));
        exitBtn.onClick.AddListener(()=>gameObject.SetActive(false));
    }
}
