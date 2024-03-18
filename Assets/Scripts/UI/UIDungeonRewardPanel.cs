using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIDungeonRewardPanel : UIPanel
{
    public TMP_Text title;
    public TMP_Text instruction;
    public Image rewardIcon;
    public TMP_Text totalAmount;

    private float duration;
    private float elaspedTime;

    private void Update()
    {
        elaspedTime += Time.deltaTime;
        if (elaspedTime > duration)
        {
            CloseUI();
        }
    }

    public void ShowUI(string title, string instruction, Sprite currencyIcon, string currencyAmount, float time)
    {
        elaspedTime = .0f;
        this.title.text = title;
        this.instruction.text = instruction;
        this.rewardIcon.sprite = currencyIcon;
        this.totalAmount.text = currencyAmount;
        gameObject.SetActive(true);
        this.duration = time;
    }
}
