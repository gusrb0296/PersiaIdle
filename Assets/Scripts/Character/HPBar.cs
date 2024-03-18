using System;
using System.Collections;
using System.Collections.Generic;
using Character.Monster;
using Keiwando.BigInteger;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] private BaseController controller;
    [SerializeField] private Slider hpBar;

    private void Start()
    {
        controller.onCurrentHPChange += UpdateHealth;
    }

    private void UpdateHealth(BigInteger current, BigInteger max)
    {
        if (!hpBar.gameObject.activeInHierarchy)
            hpBar.gameObject.SetActive(true);
        
        hpBar.value = BigInteger.ToInt32(current * 100 / max);
        
        if (current == 0)
            hpBar.gameObject.SetActive(false);
    }
}
