using System;
using System.Collections;
using Character.Monster;
using Defines;
using UnityEngine;

[Serializable]
public class AttackSystem : MonoBehaviour
{
    public LayerMask targetLayerMask { get; protected set; }
    public string targetTag { get; protected set; }
    protected CurrentStatus status;

    [Header("지정 필요")] public AttackCollider[] attackColliders;
    public AttackRangeSystem attackRangeSystem = null;

    [Header("테스트 확인용")] public AttackData[] attackDatas;
    public GameObject[] attackEffect;
    public Collider2D target;
    public bool canAttack;
    protected SpriteController spriteController;
    protected BaseController controller;


    public virtual void InitAttackSystem(PlayerData data)
    {
        status = data.status;
        targetLayerMask = data.targetLayerMask;
        targetTag = data.targetTag;

        controller = data.controller;
        spriteController = data.spriteController;

        attackDatas = new AttackData[attackColliders.Length];
        for (int i = 0; i < attackColliders.Length; ++i)
        {
            attackColliders[i].InitAttackCollider(data);
            attackDatas[i] = new AttackData(data, i == 2 ? 50 : 0, 100);
        }

        attackEffect = data.attackEffect;

        if (attackRangeSystem != null)
            attackRangeSystem.InitAttackRangeSystem(this);
    }

    public virtual void InitAttackSystem(MonsterData data)
    {
        status = data.status;
        targetLayerMask = data.targetLayerMask;
        targetTag = data.targetTag;

        controller = data.controller;
        spriteController = data.spriteController;

        attackDatas = new AttackData[attackColliders.Length];
        for (int i = 0; i < attackColliders.Length; ++i)
        {
            attackColliders[i].InitAttackCollider(data);
            attackDatas[i] = new AttackData(data, 0, 100);
        }

        if (attackRangeSystem != null)
            attackRangeSystem.InitAttackRangeSystem(this);
    }

    public void AttackEffect(int index, Vector2 direction) //, Vector2 offset, Vector3 angleOffset)
    {
        if (attackEffect.Length > 0)
        {
            // Debug.Log("Effect!!!!!!!!");
            attackEffect[index].gameObject.SetActive(true);
            attackEffect[index].transform.localRotation =
                Quaternion.Euler(0, spriteController.horizontalDirection > 0 ? 0 : 180, 0);
            // attackEffect[index].transform.localPosition = new Vector3(spriteController.horizontalDirection * offset.x, offset.y);
        }
    }

    public void AttackEvent(Vector3 offset, ECalculatePositionType type, float size, int colIndex = 0,
        int knockback = 0, float duration = 0)
    {
        // duration /= status.currentAttackSpeed;
        StartCoroutine(MoveAttackCollider(offset, type, size, colIndex, knockback, duration));
    }

    public IEnumerator MoveAttackCollider(Vector3 offset, ECalculatePositionType type, float size, int colIndex = 0,
        int knockback = 0, float duration = 0f)
    {
        float elapsedTime = 0;

        attackColliders[colIndex].SetAttackData(attackDatas[colIndex]);
        attackColliders[colIndex].gameObject.SetActive(true);

        Func<Vector3, float, float, float, Vector3> calculateFunc;
        switch (type)
        {
            case ECalculatePositionType.Circle:
                calculateFunc = CirclePosition;
                break;
            case ECalculatePositionType.Line:
                calculateFunc = LinePosition;
                break;
            case ECalculatePositionType.Outback:
                calculateFunc = OutbackPosition;
                break;
            case ECalculatePositionType.Stop:
                calculateFunc = StopPosition;
                break;
            default:
                calculateFunc = StopPosition;
                break;
        }

        while (true)
        {
            elapsedTime += Time.deltaTime * status.currentAttackSpeed;
            yield return null;

            if (elapsedTime < duration)
            {
                Vector3 newPosition = calculateFunc.Invoke(offset, duration, elapsedTime, size);
                attackColliders[colIndex].transform.localPosition = newPosition;
            }
            else
            {
                attackColliders[colIndex].gameObject.SetActive(false);
                if (attackEffect.Length > 0 && colIndex == 2)
                    attackEffect[0].gameObject.SetActive(false);
                yield break;
            }
        }
    }

    public Vector3 StopPosition(Vector3 offset, float fullTime, float passedTime, float distance)
    {
        var ret = (offset + distance * Vector3.right);
        // return new Vector3(spriteController.horizontalDirection * ret.x, ret.y, ret.z);
        return new Vector3(ret.x, ret.y, ret.z);
    }

    public Vector3 CirclePosition(Vector3 center, float fullTime, float passedTime, float radi)
    {
        float angle = (passedTime / fullTime) * 120f;
        float radian = angle * Mathf.Deg2Rad;
        var ret = (center + new Vector3(Mathf.Sin(radian), Mathf.Cos(radian), 0) * radi);
        // return new Vector3(spriteController.horizontalDirection * ret.x, ret.y, ret.z);
        return new Vector3(ret.x, ret.y, ret.z);
    }

    public Vector3 LinePosition(Vector3 start, float fullTime, float passedTime, float length)
    {
        var ret = (start + Vector3.right * (passedTime / fullTime * length));
        // return new Vector3(spriteController.horizontalDirection * ret.x, ret.y, ret.z);
        return new Vector3(ret.x, ret.y, ret.z);
    }

    public Vector3 OutbackPosition(Vector3 start, float fullTime, float passedTime, float length)
    {
        if (passedTime < fullTime / 2)
        {
            var ret = (start + Vector3.right * (length * passedTime / fullTime));
            // return new Vector3(spriteController.horizontalDirection * ret.x, ret.y, ret.z);
            return new Vector3(ret.x, ret.y, ret.z);
        }
        else
        {
            var ret = (start + Vector3.right * (length * (1 - passedTime / fullTime)));
            // return new Vector3(spriteController.horizontalDirection * ret.x, ret.y, ret.z);
            return new Vector3(ret.x, ret.y, ret.z);
        }
    }
}