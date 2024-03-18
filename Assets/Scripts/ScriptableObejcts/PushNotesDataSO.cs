using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/PushNotesData")]
public class PushNotesDataSO : ScriptableObject
{
    [SerializeField] private string title;
    [SerializeField] private string desc;
    [SerializeField] private int pushTime;
    [SerializeField] private ENormalRewardType rewardType;
    [SerializeField] private int amount;

    public string Title => title;
    public string Desc => desc;
    public int PushTime => pushTime;
    public ENormalRewardType RewardType => rewardType;
    public int Amount => amount;
}
