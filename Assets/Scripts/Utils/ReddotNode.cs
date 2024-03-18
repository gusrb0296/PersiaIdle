using System;
using System.Collections.Generic;
using Defines;
using UnityEngine;

public class ReddotNode : MonoBehaviour
{
    [Header("지정 필요")]
    public List<Reddot> dots;
    protected UIBase currentUI;

    private void Awake()
    {
        currentUI = gameObject.GetComponent<UIBase>();
        currentUI.actOnShow += CheckReddot;
        currentUI.actOnShow += AddToList;
        currentUI.actOnCallback += RemoveFromList;
    }

    private void Start()
    {
        if (currentUI.gameObject.activeInHierarchy)
        {
            CheckReddot();
            AddToList();
        }
    }

    protected void AddToList()
    {
        ReddotTree.instance.openedDots.AddLast(this);
    }

    protected void RemoveFromList()
    {
        ReddotTree.instance.openedDots.Remove(this);
    }

    protected void CheckReddot()
    {
        foreach (var dot in dots)
        {
            dot.dot.SetActive(ReddotTree.instance.reddotState[(int)dot.type]);
        }
    }

    public virtual void TurnOnOffReddot(EUpgradeType type, bool onoff)
    {
        foreach (var dot in dots)
        {
            if (dot.type == type)
            {
                dot.dot.SetActive(onoff);
            }
        }
    }
}

[Serializable]
public class Reddot
{
    public EUpgradeType type;
    public GameObject dot;
}