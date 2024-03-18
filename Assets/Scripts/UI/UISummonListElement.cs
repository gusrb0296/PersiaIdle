using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UISummonListElement : UIBase
{
    // [SerializeField] private Image frontEffect;
    // [SerializeField] private float frontEffectTime;
    // [SerializeField] private Color frontEffectInitColor;
    [SerializeField] private GameObject frontEffect;
    [SerializeField] private GameObject shakeEffect;
    [SerializeField] private Image[] backEffect;
    [SerializeField] private Image background;
    [SerializeField] private Image image;
    [SerializeField] private Animator[] animator;
    [SerializeField] private TMP_Text rarity;
    [SerializeField] private RectTransform toShake;

    [SerializeField] private GameObject[] frameEffect;
    [SerializeField] private GameObject[] powerEffect;

    private Coroutine effectJob;
    private Equipment equipment;
    private BaseSkillData skill;
    private Action onEnd;
    // private bool isEquipUI;
    private float shakeTime = 1f;

    private UISummonList uiSummonList;

    public void ShowUI<T>(UISummonList uiSummonList, T _skill) where T : BaseSkillData
    {
        // base.ShowUI();
        this.uiSummonList = uiSummonList;

        frontEffect.SetActive(true);
        this.skill = _skill;

        // background.color = Color.white;
        background.sprite = EquipmentManager.instance.GetFrame(_skill.rarity);
        if (_skill.rarity >= ERarity.Rare)
        {
            StartBackEffect(_skill.rarity);
        }

        image.sprite = SkillManager.instance.GetIcon(_skill.iconIndex);

        rarity.text = $"{Strings.rareKor[(int)_skill.rarity]}";
        // isEquipUI = false;
    }

    private void StartBackEffect(ERarity skillRarity)
    {
        backEffect[0].gameObject.SetActive(true);
        backEffect[0].color = EquipmentManager.instance.rarityColors[(int)skillRarity];
    }

    public void ShowUI(UISummonList uiSummonList, Equipment item)
    {
        // base.ShowUI();
        this.uiSummonList = uiSummonList;

        // self.localScale = Vector3.zero;
        // self.DOScale(Vector3.one, 0.1f);
        frontEffect.SetActive(true);
        equipment = item;

        // background.color = item.myColor;
        // frontEffect.color = frontEffectInitColor;
        background.sprite = EquipmentManager.instance.GetFrame(item.rarity);
        if (item.rarity >= ERarity.Rare)
        {
            StartBackEffect(item.rarity);
        }

        if (EquipmentManager.instance.images.TryGetValue(item.equipName, out Sprite sprite))
            image.sprite = sprite;

        rarity.text = $"{Strings.rareKor[(int)item.rarity]} {item.level}";

        // StartEffect();
        // StartEffect(item.eRarity);
        // frontEffect.DOFade(0f, frontEffectTime);
        // isEquipUI = true;
    }

    public void StartFrameEffect(ERarity eRarity)
    {
        if (frameEffect == null) return;

        int index = Mathf.Clamp((int)eRarity - (int)ERarity.Epic, 0, frameEffect.Length - 1);
        frameEffect[index].SetActive(true);
    }

    public void StartPowerEffect(ERarity eRarity)
    {
        if (powerEffect == null) return;

        int index = Mathf.Clamp((int)eRarity - (int)ERarity.Epic, 0, frameEffect.Length - 1);
        powerEffect[index].SetActive(true);
    }

    public void ClearEffect()
    {
        frontEffect.SetActive(false);
        foreach (var effect in frameEffect)
            effect.SetActive(false);
        foreach (var effect in powerEffect)
            effect.SetActive(false);
    }

    public override void CloseUI()
    {
        base.CloseUI();

        gameObject.SetActive(false);
        shakeEffect.SetActive(false);
        ClearEffect();
        foreach (var effect in backEffect)
            effect.gameObject.SetActive(false);
    }

    public void Shake(bool isUpgrade, Equipment toUpgrade = null, Action onEnd = null)
    {
        // TODO shake effect
        // shakeEffect.SetActive(true);
        this.onEnd = onEnd;
        StartCoroutine(ShakeEffect(isUpgrade, toUpgrade));
    }
    
    public void Shake(bool isUpgrade, BaseSkillData toUpgrade = null, Action onEnd = null)
    {
        // TODO shake effect
        // shakeEffect.SetActive(true);
        StartCoroutine(ShakeEffect(isUpgrade, toUpgrade));
        this.onEnd = onEnd;
    }
    
    private IEnumerator ShakeEffect(bool isUpgrade, Equipment toUpgrade = null)
    {
        float passedTime = .0f;

        // do
        // {
        var tween = toShake.DOShakeAnchorPos(shakeTime, Vector2.right * SummonManager.instance.shakePower,
            SummonManager.instance.shakeVibrato, 0, false, false);

        while (passedTime < shakeTime)
        {
            passedTime += Time.deltaTime;
            // TODO if (skip) break
            yield return null;
            if (uiSummonList.isSkip) break;
        }

        tween.Kill();
        ClearEffect();
        shakeEffect.SetActive(false);

        yield return null;

        if (isUpgrade)
        {
            shakeEffect.SetActive(true);
            ShowUI(uiSummonList, toUpgrade);
        }
        // if (Random.Range(0f, 1f) < SummonManager.instance.shakeProbability)
        // {
        //     
        //     if (isEquipUI)
        //     {
        //         // upgrade item
        //         var upgradeItem = EquipmentManager.instance.TryGetEquipment(equipment.type, 4 * ((int)equipment.rarity + 1) + equipment.level - 1);
        //         ShowUI(upgradeItem);
        //     }
        //     else
        //     {
        //         // Upgrade Skill
        //         var upgradeSkill = SummonManager.instance.SummonSkillsInRarity(skill.rarity + 1);
        //         ShowUI(upgradeSkill);
        //     }
        //     // frontEffect.SetActive(false);
        //
        //     if (isEquipUI && equipment.rarity >= ERarity.Legendary)
        //         break;
        //     if (!isEquipUI && skill.rarity >= ERarity.Epic)
        //         break;
        //     
        //     yield return new WaitForSeconds(1.0f);
        // }
        // else
        // {
        //     break;
        // }
        //
        // passedTime = .0f;
        // } while (Random.Range(0f, 1f) < SummonManager.instance.shakeProbability);

        onEnd?.Invoke();
    }
    
    private IEnumerator ShakeEffect(bool isUpgrade, BaseSkillData toUpgrade = null)
    {
        float passedTime = .0f;
        
        var tween = toShake.DOShakeAnchorPos(shakeTime, Vector2.right * SummonManager.instance.shakePower,
            SummonManager.instance.shakeVibrato, 0, false, false);

        while (passedTime < shakeTime)
        {
            passedTime += Time.deltaTime;
            yield return null;
            if (uiSummonList.isSkip) break;
        }

        tween.Kill();
        ClearEffect();
        shakeEffect.SetActive(false);

        yield return null;

        if (isUpgrade)
        {
            shakeEffect.SetActive(true);
            ShowUI(uiSummonList, toUpgrade);
        }

        onEnd?.Invoke();
    }
}