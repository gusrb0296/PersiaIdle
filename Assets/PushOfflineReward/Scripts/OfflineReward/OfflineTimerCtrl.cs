using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Keiwando.BigInteger;

public class OfflineTimerCtrl : MonoBehaviour
{
    public static OfflineTimerCtrl instance;
    
    private float minTime = 60;
    private float maxTime = 28800;

    private int secondPerKill = 120;

    private float timePassed;

    private UIOffLineReward ui_offLineReward;

    private DateTime startTime;
    private string startTimestr;

    private void Awake()
    {
        instance = this;
    }

    public void InitOfflineTimer()
    {
        SetReferences();
        InitKey();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) TimeReset();
        else RefilKey();
    }
    
    private void SetReferences()
    {
        ui_offLineReward = UIManager.instance.TryGetUI<UIOffLineReward>();
        ui_offLineReward.Initialize();
    }

    private void InitKey()
    {
        if (!PlayerPrefs.HasKey("OfflineTimerStr" + Application.productName)) TimeReset();
        else RefilKey();
    }

    public void RefilKey()
    {
        TimeLoad();

        TimeSpan ts = DateTime.Now - startTime;
        timePassed = (float)ts.TotalSeconds;

        ManagePushRewards();

        if (timePassed >= minTime)   //10 Minutes 600  //30 Minutes 1800  // 3시간 10800  //4시간 14400
        {
            OfflinePanelOpen();
            TimeReset();
            Debug.Log("OfflineTimer" + " + " + timePassed);
        }
        else
        {
            Debug.Log("OfflineTimer" + " + " + timePassed);
        }
    }

    private void TimeLoad()
    {
        startTimestr = PlayerPrefs.GetString("OfflineTimerStr" + Application.productName);
        startTime = DateTime.Parse(startTimestr);
    }

    public void TimeReset()
    {
        startTime = DateTime.Now;
        startTimestr = startTime.ToString();
        PlayerPrefs.SetString("OfflineTimerStr" + Application.productName, startTimestr);
    }

    // 오프라인 타임이 특정시간 이상 지났다면 보상창 띄우기
    private void OfflinePanelOpen()
    {
        timePassed = Mathf.Min(timePassed, maxTime);
        int intTime = Mathf.FloorToInt(timePassed);
        int killCount = intTime / secondPerKill;

        ui_offLineReward.ShowUI(killCount, intTime);
    }

    // Ok확인 보상받고 패널끄기 
    public void OnClickBtn_Ok()
    {
        TimeReset(); // 타이머 리셋
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    private void ManagePushRewards()
    {
        // List<PushNotesDataSO> pushDatas = PushNotificationManager.instance.GetUnrecievedRewardDatas((int)timePassed);
        List<PushNotesDataSO> pushDatas = PushNotificationManager.instance.GetUnrecievedRewardDatas((int)timePassed/3600);
        List<Reward> rewards = new List<Reward>();

        foreach (PushNotesDataSO data in pushDatas)
        {
            PushNotificationManager.instance.SetRewardRecieved(data.name);
            if (data.RewardType == ENormalRewardType.None) continue;

            rewards.Add(new Reward(data.RewardType, data.Amount));
        }

        if (rewards.Count > 0) PushNotificationManager.instance.GiveReward(rewards);
    }
}

public class Reward
{
    public ENormalRewardType type;
    public BigInteger amount;

    public Reward(ENormalRewardType type, BigInteger amount)
    {
        this.type = type;
        this.amount = amount;
    }
}
