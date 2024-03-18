using System;
using System.Collections;
using System.Collections.Generic;
using Character.Monster;
using UnityEngine;
using UnityEngine.Serialization;

public class SpriteController : MonoBehaviour
{
    #region Sprite order

    [SerializeField] private SpriteRenderer mainSprite;
    public static int defaultOrder = 500;

    #endregion

    #region Hit effect color

    private Coroutine changeColorJob;

    #endregion

    #region Hit effect animator

    [SerializeField] private Animator hitAnimator;
    [SerializeField] private string hitParameter;
    private int hitHash;
    private Coroutine hitEffectJob;

    #endregion

    /// <summary>
    /// right = 1, left = -1
    /// </summary>
    public sbyte horizontalDirection { get; protected set; } = 1;

    public int orderInPlace { get; private set; }

    private Vector3 originScale;

    private void Update()
    {
        //SpriteOrder(transform.position);
    }

    public void InitializeController(BaseController controller)
    {
        originScale = mainSprite.transform.localScale;
        hitHash = Animator.StringToHash(hitParameter);
        controller.onMove += RotateSprite;
        controller.onAttack += RotateSprite;
        controller.onHit += SpriteHitColorChange;
        controller.onHit += SpriteHitEffect;
    }

    private void RotateSprite(Vector2 direction)
    {
        var dec = Vector2.Dot(direction, Vector2.right);
        if (dec < 0)
        {
            // mainSprite.flipX = true;
            horizontalDirection = -1;
        }
        else if (dec > 0)
        {
            // mainSprite.flipX = false;
            horizontalDirection = 1;
        }

        mainSprite.transform.localScale =
            new Vector3(horizontalDirection * originScale.x, originScale.y, originScale.z);
    }

    public void SpriteOrder(Vector2 position)
    {
        orderInPlace = defaultOrder - Mathf.RoundToInt(position.y * 100);
        mainSprite.sortingOrder = orderInPlace;
    }

    public void SpriteHitColorChange(Vector2 direction, float knockback)
    {
        if (!ReferenceEquals(changeColorJob, null))
            StopCoroutine(changeColorJob);
        if (gameObject.activeInHierarchy)
            changeColorJob = StartCoroutine(ChangeColor(Color.red, 0.1f));
    }

    private IEnumerator ChangeColor(Color toColor, float time = 1.0f)
    {
        Color origin = Color.white;
        Color diff = (toColor - origin) / time;
        float spendTime = 0f;
        while (spendTime < time)
        {
            mainSprite.color += diff * Time.deltaTime;
            spendTime += Time.deltaTime;
            yield return null;
        }

        spendTime = 0f;
        diff = (origin - mainSprite.color) / time;
        while (spendTime < time)
        {
            mainSprite.color += diff * Time.deltaTime;
            spendTime += Time.deltaTime;
            yield return null;
        }

        mainSprite.color = Color.white;
    }

    public void SpriteHitEffect(Vector2 direction, float knockback)
    {
        if (!ReferenceEquals(hitEffectJob, null))
        {
            hitAnimator.SetBool(hitHash, false);
            StopCoroutine(HitEffect());
        }

        if (gameObject.activeInHierarchy)
            hitEffectJob = StartCoroutine(HitEffect());
    }

    private IEnumerator HitEffect()
    {
        float elapsedTime = .0f;
        hitAnimator.gameObject.SetActive(true);
        hitAnimator.SetBool(hitHash, true);
        while (elapsedTime < .2f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        hitAnimator.SetBool(hitHash, false);
        hitAnimator.gameObject.SetActive(false);
    }

    private float GetNormalizeTime()
    {
        return hitAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}