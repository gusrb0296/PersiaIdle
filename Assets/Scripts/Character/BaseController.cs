using System;
using Keiwando.BigInteger;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public event Action<Vector2, float> onHit;
    public event Action<Vector2> onAttack;
    public event Action<Vector2> onMove;
    public event Action onDeathStart;
    public event Action onDeathEnd;
    public event Action onIdle;
    public event Action<BigInteger, BigInteger> onCurrentHPChange;
    public event Action<BigInteger> onCurrentMPChange;

    // private int test;

    public void CallDeathStart()
    {
        // ++test;
        // Debug.Log($"{gameObject.name} {test}");
        onDeathStart?.Invoke();
    }

    public void CallCurrentHPChange(BigInteger current, BigInteger max)
    {
        onCurrentHPChange?.Invoke(current, max);
    }
    public void CallCurrentMPChange(BigInteger value)
    {
        onCurrentMPChange?.Invoke(value);
    }
    public void CallHit(Vector2 direction, float knockBack=0f)
    {
        onHit?.Invoke(direction, knockBack);
    }

    public void CallAttack(Vector2 direction)
    {
        onAttack?.Invoke(direction);
    }

    public virtual void CallMove(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.1f)
        {
            CallIdle();
        }
        else
            onMove?.Invoke(direction);
    }

    public void CallDeathEnd()
    {
        // test = 0;
        onDeathEnd?.Invoke();
    }

    public void CallIdle()
    {
        onIdle?.Invoke();
    }
}
