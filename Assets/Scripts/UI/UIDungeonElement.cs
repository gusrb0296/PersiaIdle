using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

public class UIDungeonElement : UIBase
{
    public Image dungeonIcon;
    public TMP_Text dungeonSubTitle;
    public Image rewardIcon;
    public TMP_Text rewardAmount;
    public Image invitationIcon;
    public TMP_Text invitationAmount;
    public Button enterBtn;
    private DungeonData dungeonData;

    private RectTransform btnRect;

    public UILock uiLock;

    public ReddotNode reddotNode { get; protected set; }

    public override UIBase InitUI(UIBase parent)
    {
        base.InitUI(parent);
        InitializeBtns();
        reddotNode = gameObject.GetComponent<ReddotNode>();
        return this;
    }

    public RectTransform GetButtonRect()
    {
        if (ReferenceEquals(btnRect, null))
            btnRect = enterBtn.GetComponent<RectTransform>();
        return btnRect;
    }

    public void ShowUI(UIDungeonPanel parent, DungeonData data)
    {
        gameObject.SetActive(true);
        this.dungeonData = data;
        this.parent = parent;

        dungeonIcon.sprite = data.dungeonImage;
        dungeonSubTitle.text = Strings.dungeonToKOR[(int)data.dungeonType] + CustomText.SetColor($" Lv.{data.dungeonLevel}", data.invitationType);;
        rewardIcon.sprite = CurrencyManager.instance.GetIcon((ECurrencyType)data.rewardType);
        rewardAmount.text = data.GetTotalReward().ChangeToShort();
        invitationIcon.sprite = CurrencyManager.instance.GetIcon(data.invitationType);
        invitationAmount.text = CurrencyManager.instance.GetCurrency(data.invitationType).ChangeToShort();
    }
    
    public override void CloseUI()
    {
        base.CloseUI();
        
        gameObject.SetActive(false);
    }

    public void InitializeBtns()
    {
        enterBtn.onClick.AddListener(EnterDungeon);
    }

    public void EnterDungeon()
    {
        UIManager.instance.TryGetUI<UIDungeonPanel>().ShowPopup(dungeonData);
        // StageManager.instance.EnterTheDungeon(data);
    }

    public void Lock(int questIndex)
    {
        uiLock.LockUI(questIndex);
    }

    public void Unlock()
    {
        uiLock.UnlockUI();
    }
}
