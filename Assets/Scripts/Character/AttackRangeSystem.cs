using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRangeSystem : MonoBehaviour
{
    private AttackSystem attackSystem;
    private LayerMask targetLayer;
    private string targetTag;
    public int enemyCount = 0;
    private Vector2 size;

    private void Awake()
    {
        size = gameObject.GetComponent<BoxCollider2D>().size;
    }

    public void InitAttackRangeSystem(AttackSystem attackSystem)
    {
        this.attackSystem = attackSystem;
        targetLayer = attackSystem.targetLayerMask;
        targetTag = attackSystem.targetTag;
        enemyCount = 0;
    }

    public GameObject GetEnemyInArea()
    {
        var colliders = Physics2D.OverlapBoxAll(transform.position, size, 0f, targetLayer);
        GameObject nearest = null;
        foreach (var target in colliders)
        {
            if (ReferenceEquals(nearest, null) ||
                Vector2.Distance(nearest.transform.position, transform.position)
                > Vector2.Distance(target.transform.position, transform.position))
            {
                nearest = target.gameObject;
            }
        }

        return nearest;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer) == ((1 << collision.gameObject.layer) & targetLayer))// || collision.CompareTag(targetTag))
        {
            enemyCount++;
            attackSystem.canAttack = enemyCount > 0;
        }
        // if (targetLayer.value == (1 << LayerMask.NameToLayer("Monster")))
        //     Debug.Log($"Enter {collision.gameObject.layer} / {enemyCount}");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer) == ((1 << collision.gameObject.layer) & targetLayer))// || collision.CompareTag(targetTag))
        {
            enemyCount = enemyCount <= 0 ? 0 : enemyCount - 1;
            attackSystem.canAttack = enemyCount > 0;
        }
        // if (targetLayer.value == (1 << LayerMask.NameToLayer("Monster")))
        //     Debug.Log($"Exit {collision.gameObject.layer} / {enemyCount}");
    }
}
