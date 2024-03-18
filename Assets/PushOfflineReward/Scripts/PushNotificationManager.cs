using UnityEngine;
using UnityEngine.Android;
using Unity.Notifications.Android;
using System;
using System.Collections.Generic;
using Defines;
using Keiwando.BigInteger;

public class PushNotificationManager : Singleton<PushNotificationManager>
{
    Dictionary<string, PushNotesDataSO> dataDic;
    Dictionary<string, bool> rewardRecieved;

    public int hasPermissionChecked
    {
        get { return PlayerPrefs.GetInt("hasPermissionChecked_" + Application.productName, 0); }
        set { PlayerPrefs.SetInt("hasPermissionChecked_" + Application.productName, value); }
    }

#if UNITY_EDITOR || UNITY_ANDROID
    // private void Start()
    // {
    //     Initialize();
    // }

    public void Initialize()
    {
        SetCollections();

        // // 최초 한 번만 권한 체크
        // if (hasPermissionChecked == 0)
        // {
        //     CheckNotificationPermission();
        //     hasPermissionChecked = 1;
        // }
        //
        // // 게임시작 모든 알람 지우기
        // AndroidNotificationCenter.CancelAllNotifications();
        // AndroidNotificationCenter.CancelAllScheduledNotifications();

        LoadDatas();
    }

    public void SetPermission()
    {
        // 최초 한 번만 권한 체크
        if (hasPermissionChecked == 0)
        {
            CheckNotificationPermission();
            hasPermissionChecked = 1;
        }

        // 게임시작 모든 알람 지우기
        AndroidNotificationCenter.CancelAllNotifications();
        AndroidNotificationCenter.CancelAllScheduledNotifications();
    }

    private void SetCollections()
    {
        dataDic = new Dictionary<string, PushNotesDataSO>();
        rewardRecieved = new Dictionary<string, bool>();
    }

    private void LoadDatas()
    {
        PushNotesDataSO[] datas = Resources.LoadAll<PushNotesDataSO>("ScriptableObjects/PushNotesDataSO");

        foreach(PushNotesDataSO data in datas)
        {
            dataDic[data.name] = data;
        }

        LoadRewardRecieved();
    }

    private void LoadRewardRecieved()
    {
        foreach (KeyValuePair<string, PushNotesDataSO> kvp in dataDic)
        {
            rewardRecieved[kvp.Key] = ES3.KeyExists($"PushRewardRecieved_{kvp.Key}") ? ES3.Load<bool>($"PushRewardRecieved_{kvp.Key}") : false;
        }
    }

    private void CheckNotificationPermission()
    {
        // 푸시 알림 권한이 허용되어 있는지 확인
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            // 권한이 허용되지 않은 경우 권한 요청
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            // 알림 예약
            if (Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                SendLocalNotification();
            }

            SaveRewardRecieved();
        }
        else
        {
            // 알림 예약 제거
            AndroidNotificationCenter.CancelAllNotifications();
            AndroidNotificationCenter.CancelAllScheduledNotifications();
        }
    }


    private void SendLocalNotification()
    {
        // Android에서만 사용되는 푸시 채널 설정
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Channel",
            Importance = Importance.Default,
            Description = "Description",
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        if (dataDic == null) Initialize();

        foreach(KeyValuePair<string, PushNotesDataSO> kvp in dataDic)
        {
            if (rewardRecieved[kvp.Key]) continue;

            Debug.Log($"Push: {kvp.Key}");
            // AndroidNotificationCenter.SendNotification(new AndroidNotification(kvp.Value.Title, kvp.Value.Desc, DateTime.Now.AddSeconds(kvp.Value.PushTime)), "channel_id");
            AndroidNotificationCenter.SendNotification(new AndroidNotification(kvp.Value.Title, kvp.Value.Desc, DateTime.Now.AddHours(kvp.Value.PushTime)), "channel_id");
        }
    }
#endif

    public List<PushNotesDataSO> GetUnrecievedRewardDatas(int hour)
    {
        List<PushNotesDataSO> pushDatas = new List<PushNotesDataSO>();

        foreach(KeyValuePair<string, PushNotesDataSO> kvp in dataDic)
        {
            if (!rewardRecieved[kvp.Key] && kvp.Value.PushTime <= hour)
            {
                pushDatas.Add(kvp.Value);
            }
        }

        return pushDatas;
    }

    public void SetRewardRecieved(string dataName)
    {
        rewardRecieved[dataName] = true;
    }

    private void SaveRewardRecieved()
    {
        if (rewardRecieved == null) Initialize();
        
        foreach (KeyValuePair<string, bool> kvp in rewardRecieved)
        {
            ES3.Save($"PushRewardRecieved_{kvp.Key}", kvp.Value);
        }
    }

    public void GiveReward(List<Reward> rewards)
    {
        // TODO add item to player
        foreach (var reward in rewards)
        {
            switch (reward.type)
            {
                case ENormalRewardType.Gold:
                case ENormalRewardType.Dia:
                case ENormalRewardType.EnhanceStone:
                case ENormalRewardType.AwakenStone:
                case ENormalRewardType.WeaponSummonTicket:
                case ENormalRewardType.ArmorSummonTicket:
                case ENormalRewardType.GoldInvitation:
                case ENormalRewardType.AwakenInvitation:
                case ENormalRewardType.EnhanceInvitation:
                    CurrencyManager.instance.AddCurrency((ECurrencyType)reward.type, reward.amount);
                    UIManager.instance.TryGetUI<UIOffLineReward>().AddReward(reward);
                    break;
                case ENormalRewardType.Exp:
                    PlayerManager.instance.levelSystem.EarnExp(reward.amount);
                    UIManager.instance.TryGetUI<UIOffLineReward>().AddReward(reward);
                    break;
                case ENormalRewardType.Weapon:
                {
                    var item = EquipmentManager.instance.TryGetEquipment(EEquipmentType.Weapon, BigInteger.ToInt32(reward.amount));
                    if (!item.IsOwned)
                    {
                        item.IsOwned = true;
                        item.Save(ESaveType.IsOwned);
                        PlayerManager.instance.ApplyOwnedEffect(item);
                    }
                    UIManager.instance.TryGetUI<UIOffLineReward>().AddReward(reward);
                    break;
                }
                case ENormalRewardType.Armor:
                {
                    var item = EquipmentManager.instance.TryGetEquipment(EEquipmentType.Armor, BigInteger.ToInt32(reward.amount));
                    if (!item.IsOwned)
                    {
                        item.IsOwned = true;
                        item.Save(ESaveType.IsOwned);
                        PlayerManager.instance.ApplyOwnedEffect(item);
                    }
                    UIManager.instance.TryGetUI<UIOffLineReward>().AddReward(reward);
                    break;
                }
            }
        }
    }
}
