using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

public class UIDamage : UIBase
{
    [SerializeField] private float baseScale;
    [SerializeField] private SpriteRenderer image;
    [SerializeField] private float duration;
    [SerializeField] private float speed;
    [SerializeField] private float maxScale;
    private float passedTime;

    public override UIBase InitUI(UIBase _parent)
    {
        base.InitUI(_parent);
        return this;
    }

    public virtual void ShowUI(Vector3 position, BigInteger damage, bool isCrit = false)
    {
        Self.position = position;
        if (textDatas.Length > 0)
        {
            textDatas[0].text = damage.ChangeToShort();
            textDatas[0].color = isCrit ? Color.yellow : Color.white;
        }

        Self.localScale = baseScale * Vector3.one;

        var col = image.color;
        col.a = 1;
        image.color = col;

        foreach (var text in textTitles)
        {
            col = text.color;
            col.a = 1;
            text.color = col;
        }

        foreach (var text in textDatas)
        {
            col = text.color;
            col.a = 1;
            text.color = col;
        }
    }

    private void OnEnable()
    {
        passedTime = .0f;
    }

    private void Update()
    {
        passedTime += Time.deltaTime;
        Self.position += Vector3.up * (speed * Time.deltaTime);
        if (passedTime > duration * 2)
        {
            CloseUI();
        }
        else if (passedTime > duration)
        {
            passedTime += Time.deltaTime;

            var value = ToSmall(passedTime - duration, duration);
            var col = image.color;
            col.a = value;
            image.color = col;

            foreach (var text in textTitles)
            {
                col = text.color;
                col.a = value;
                text.color = col;
            }

            foreach (var text in textDatas)
            {
                col = text.color;
                col.a = value;
                text.color = col;
            }
        }
        else if (passedTime < duration / 3)
        {
            var value = ToSmall(passedTime, duration / 3);
            Self.localScale = Vector3.one * (baseScale + maxScale * value);
        }
    }

    private float ToSmall(float time, float length)
    {
        return 1 - time / length;
    }

    public override void CloseUI()
    {
        base.CloseUI();
        gameObject.SetActive(false);
    }
}