using System;
using System.Collections.Generic;
using Character.Monster;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    protected AttackSystem attackSystem;

    protected LayerMask targetLayer;
    protected string targetTag;
    protected AttackData attackData;
    protected bool isContinuous;
    protected int attackCount;
    private int count;
    private float elapsedTime;
    private LinkedList<BaseData> tickList;

    private Transform attacker;

    private void Awake()
    {
        tickList = new LinkedList<BaseData>();
    }

    public void ClearTickList()
    {
        if (isContinuous)
            tickList.Clear();
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        
        if (isContinuous && attackCount > count && elapsedTime / attackData.tickUnitTime > count)
        {
            ++count;
            var tick = tickList.First;
            while (!ReferenceEquals(tick, null))
            {
                if (!tick.Value.IsDead)
                    tick.Value.health.SubstractHP(tick.Value.transform.position - transform.position, attackData);
                tick = tick.Next;
            }
        }
    }

    public virtual void InitAttackCollider(BaseData character)
    {
        attackSystem = character.attackSystem;
        targetLayer = character.targetLayerMask;
        targetTag = character.targetTag;
        attacker = character.transform;
    }

    public virtual void InitAttackCollider(MonsterData monster)
    {
        attackSystem = monster.attackSystem;
        targetLayer = monster.targetLayerMask;
        targetTag = monster.targetTag;
        attacker = monster.transform;
    }

    public virtual void SetAttackData(AttackData attackData)
    {
        this.attackData = attackData;
        isContinuous = attackData.isContinuous;
        attackCount = attackData.attackCount;
        elapsedTime = .0f;
        count = 0;
    }

    // private void Update()
    // {
    //     if (isContinuous)
    //     {
    //         preCount = count;
    //         elapsedTime += Time.deltaTime;
    //         if (count < attackCount && elapsedTime / 0.2f > count)
    //             ++count;
    //     }
    // }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (isContinuous)
        {
            if ((1 << collision.gameObject.layer) ==
                ((1 << collision.gameObject.layer) & targetLayer)) // || collision.CompareTag(targetTag))
            {
                var data = collision.GetComponent<BaseData>();
                // 몬스터에게 공격하였으나, data가 null인 버그 발생. 게임 오프젝트를 지우는 과정으로 인한 버그로 생각됨.
                if (data.IsDead)
                {
                    // Debug.Log($"{transform.parent.name} is Already Dead");
                    return;
                }

                tickList.AddLast(data);
            }
        }
        else
        {
            // Debug.Log($"tag : {targetTag}/{collision.tag}. layer : {targetLayer.value}/{1<<collision.gameObject.layer}");
            if ((1 << collision.gameObject.layer) ==
                ((1 << collision.gameObject.layer) & targetLayer)) // || collision.CompareTag(targetTag))
            {
                var data = collision.GetComponent<BaseData>();
                // 몬스터에게 공격하였으나, data가 null인 버그 발생. 게임 오프젝트를 지우는 과정으로 인한 버그로 생각됨.
                if (data.IsDead)
                {
                    // Debug.Log($"{transform.parent.name} is Already Dead");
                    return;
                }

                data.health.SubstractHP(data.transform.position - attacker.position, attackData);
            }
        }
    }

    // protected virtual void OnTriggerStay2D(Collider2D collision)
    // {
    //     if (!isContinuous)
    //         return;
    //
    //     if (count > preCount)
    //     {
    //         if ((1 << collision.gameObject.layer) == ((1 << collision.gameObject.layer) & targetLayer)) // || collision.CompareTag(targetTag))
    //         {
    //             var data = collision.GetComponent<BaseData>();
    //             // 몬스터에게 공격하였으나, data가 null인 버그 발생. 게임 오프젝트를 지우는 과정으로 인한 버그로 생각됨.
    //             if (data.IsDead)
    //             {
    //                 // Debug.Log($"{transform.parent.name} is Already Dead");
    //                 return;
    //             }
    //
    //             data.health.SubstractHP(data.transform.position - transform.position, attackData.GetDamage(),
    //                 attackData.GetKnockBack());
    //         }
    //     }
    // }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isContinuous)
        {
            if ((1 << collision.gameObject.layer) == ((1 << collision.gameObject.layer) & targetLayer))
            {
                var data = collision.GetComponent<BaseData>();

                tickList.Remove(data);
            }
        }
    }
}