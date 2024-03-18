using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIBottomMenuCtrl : UIBase
{
    [Header("버튼 (왼쪽부터 순서대로 넣을 것)")]
    public Button[] buttons;
    
    [SerializeField] private Button dungeonExitButton;
    [SerializeField] private GameObject dungeonButtonAll;

    private List<UIPanel> panels;
    
    private void Start()
    {
        panels = new List<UIPanel>();
        
        UIPanel panel = UIManager.instance.TryGetUI<UIGrowthPanel>();
        if (panel != null)
            panels.Add(panel);
        panel = UIManager.instance.TryGetUI<UISummonPanel>();
        if (panel != null)
            panels.Add(panel);
        panel = UIManager.instance.TryGetUI<UIEquipmentPanel>();
        if (panel != null)
            panels.Add(panel);
        panel = UIManager.instance.TryGetUI<UISkillPanel>();
        if (panel != null)
            panels.Add(panel);
        panel = UIManager.instance.TryGetUI<UIDungeonPanel>();
        if (panel != null)
            panels.Add(panel);
        
        // 각 버튼에 이벤트 리스너 할당
        for (int i = 0; i < buttons.Length; ++i)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnButtonClicked(index));
        }
        
        dungeonExitButton.onClick.AddListener(CallConfirm);
    }

    private void CallConfirm()
    {
        var ui = UIManager.instance.TryGetUI<UIConfirm>();
        ui.ShowUI(Strings.exitDungeon, StageManager.instance.RetireDungeon);
        ui.AddOnOK(() => ActivateDungeonBtn(false));
    }

    // 버튼 클릭 시 호출되는 메서드
    private void OnButtonClicked(int index)
    {
        // 모든 패널을 순회하면서 상태 설정
        for (int i = 0; i < panels.Count; i++)
        {
            var checkPanel = panels[i];
            if (i == index)
                checkPanel.ShowUI();
            else
            {
                if (checkPanel.gameObject.activeInHierarchy)
                    checkPanel.CloseUI();
            }
        }
    }

    public void InteractableChange(bool onoff)
    {
        foreach (var btn in buttons)
        {
            btn.interactable = onoff;
        }
    }

    public void ActivateDungeonBtn(bool onoff)
    {
        dungeonButtonAll.SetActive(onoff);

        foreach (var btn in buttons)
        {
            btn.gameObject.SetActive(!onoff);
        }
    }
}
