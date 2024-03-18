using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UILock : MonoBehaviour
{
    public bool isQuestLock;
    public int unlockQuestIndex;
    public string message;
    [SerializeField] private Button lockBtn;
    [FormerlySerializedAs("lockObjToDisappear")] [SerializeField] private GameObject lockObjToAppear;
    public ELockType type;

    private void Start()
    {
        if (!ReferenceEquals(lockBtn,null))
        {
            InitializeBtn();
        }
        
        if (!isQuestLock) return;
        
        var id = QuestManager.instance.currentQuest.GetID();
        
        if (id < unlockQuestIndex || (id == unlockQuestIndex && !QuestManager.instance.currentQuest.isComplete))
            QuestManager.instance.quests[unlockQuestIndex].onStart += UnlockUI;
        else
            UnlockUI();
    }

    public void UnlockUI(BaseAchievement quest)
    {
        UnlockUI();
    }

    public void LockUI(int questIndex, string message = null)
    {
        switch (type)
        {
            case ELockType.LockIcon:
                gameObject.SetActive(true);
                break;
            case ELockType.Appear:
                lockObjToAppear.SetActive(false);
                break;
        }

        if (questIndex != 0)
        {
            isQuestLock = true;
            unlockQuestIndex = questIndex;
        }
        else
        {
            this.message = message;
        }
    }

    public void UnlockUI()
    {
        switch (type)
        {
            case ELockType.LockIcon:
                gameObject.SetActive(false);
                // Destroy(gameObject);
                break;
            case ELockType.Appear:
                lockObjToAppear.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void InitializeBtn()
    {
        lockBtn.onClick.AddListener(ShowLockMessage);
    }

    private void ShowLockMessage()
    {
        if (isQuestLock)
            MessageUIManager.instance.ShowCenterMessage($"퀘스트 {unlockQuestIndex+1} 도달 시 해제됩니다.");
        else
            MessageUIManager.instance.ShowCenterMessage(message);
    }
}
