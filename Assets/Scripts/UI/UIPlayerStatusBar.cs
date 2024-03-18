using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerStatusBar : UIBase
{
    [SerializeField] protected Image userImage;
    [SerializeField] protected Button userStatusBtn;
    [SerializeField] protected TMP_Text userName;
    [SerializeField] protected TMP_Text level;

    [SerializeField] protected Slider expSlider;
    [SerializeField] protected TMP_Text expCount;

    protected void Awake()
    {
        InitializeBtn();
    }

    public override void ShowUI()
    {
        base.ShowUI();

        userImage.sprite = PlayerManager.instance.GetUserImage();
        userName.text = PlayerManager.instance.userName;
        DisplayLevelUpdate(PlayerManager.instance.levelSystem.Level);
        SubscribeEvents();
    }

    public override void CloseUI()
    {
        base.CloseUI();
        UnsubscribeEvents();
    }

    private void InitializeBtn()
    {
        userStatusBtn.onClick.AddListener(() => UIManager.instance.TryGetUI<UIPlayerStatusPanel>()?.ShowUI());
        expSlider.wholeNumbers = true;
        expSlider.maxValue = 10000;
    }

    private void SubscribeEvents()
    {
        PlayerManager.instance.levelSystem.onLevelChange += DisplayLevelUpdate;
        // TODO subscribe exp
        PlayerManager.instance.levelSystem.onEarnEXP += DisplayExpUpdate;
    }

    private void UnsubscribeEvents()
    {
        PlayerManager.instance.levelSystem.onLevelChange -= DisplayLevelUpdate;
        PlayerManager.instance.levelSystem.onEarnEXP -= DisplayExpUpdate;
    }
    
    public void DisplayLevelUpdate(int currentLv) { level.text = currentLv.ToString(); }
    public void DisplayExpUpdate(BigInteger current, BigInteger full)
    {
        var res = BigInteger.ToInt32((current * 10000) / full);
        expSlider.value = res;
        expCount.text = $"EXP  {current} / {full}  ({(res / 100f):F2}%)";
    }
}
