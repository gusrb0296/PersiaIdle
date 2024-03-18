using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIObtainMessage : UIBase
{
    [SerializeField] private Image currencyIcon;
    [SerializeField] private TMP_Text currencyText;
    [SerializeField] private Image background;

    [Header("확인용")] public ECurrencyType currencyType;
    public string currencyAmount;
    public float showDuration;

    private Coroutine work;

    private Color iconOrigin;
    private Color textOrigin;
    private Color backOrigin;

    protected void Awake()
    {
        iconOrigin = currencyIcon.color;
        textOrigin = currencyText.color;
        backOrigin = background.color;
    }

    public void ShowUI(ECurrencyType type, string amount, float duration, float fadeDuration)
    {
        base.ShowUI();
        currencyType = type;
        currencyAmount = amount;
        this.showDuration = duration;

        if (work != null)
        {
            StopCoroutine(work);
        }

        work = StartCoroutine(StartShow(CurrencyManager.instance.GetIcon(type), Strings.currencyToKOR[(int)type],
            amount, duration, fadeDuration));
    }

    private IEnumerator StartShow(Sprite icon, string typeText, string amount, float duration, float fade)
    {
        float elapsedTime = .0f;

        currencyIcon.sprite = icon;
        currencyText.text = $"{typeText} x{amount}";

        yield return null;

        Color iconColor = iconOrigin;
        Color textColor = textOrigin;
        Color backColor = backOrigin;

        currencyIcon.color = iconColor;
        currencyText.color = textColor;
        background.color = backColor;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // yield return new WaitForSeconds(duration);

        elapsedTime = .0f;


        while (elapsedTime < fade)
        {
            elapsedTime += Time.deltaTime;
            currencyIcon.color = iconColor - new Color(0, 0, 0, iconColor.a / fade * elapsedTime);
            currencyText.color = textColor - new Color(0, 0, 0, textColor.a / fade * elapsedTime);
            background.color = backColor - new Color(0, 0, 0, backColor.a / fade * elapsedTime);
            yield return null;
        }

        CloseUI();
        currencyIcon.color = iconColor;
        currencyText.color = textColor;
        background.color = backColor;
    }

    public override void CloseUI()
    {
        base.CloseUI();

        gameObject.SetActive(false);
    }
}