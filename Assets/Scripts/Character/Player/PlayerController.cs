using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Defines;
using Unity.XR.GoogleVr;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : BaseController
{
    public event Action<Vector2, Vector2, float> onDashStart;
    public event Action onDashEnd;
    public event Action<Vector3, string> onActiveSkill;
    public event Action<float, string> onBuffSkill;
    public event Action<float> onSpecial;

    Vector2 paddingPosition = new Vector2(1f, 0);

    private bool isDash;
    
    public float dashCoolTime;
    private float elapsedTime;
    public float dashDuration;

    private void Update()
    {
        elapsedTime += Time.deltaTime;
    }

    public void CallDashStart(Vector2 start, Vector2 end, float time)
    {
        onDashStart?.Invoke(start, end, time);
    }

    public void CallDashEnd()
    {
        onDashEnd?.Invoke();
    }

    public bool CallSkill(int slot)
    {
        var skillData = PlayerManager.instance.EquippedSkill[slot];
        if (skillData.skillType == ESkillType.Active)
        {
            if (SkillManager.instance.CallActiveSkill(skillData.skillName, out Vector3 direction))
            {
                onActiveSkill?.Invoke(direction, skillData.skillName);
                return true;
            }
        }
        else if (skillData.skillType == ESkillType.Buff)
        {
            if (SkillManager.instance.CallBuffSkill(skillData.skillName, out float duration))
            {
                onBuffSkill?.Invoke(duration, skillData.skillName);
                return true;
            }
        }
        else
        {
            Debug.LogWarning("스킬 타입 확인 필요!");
        }
        
        return false;
    }

    public void CallSpecialSkill()
    {
        if (SkillManager.instance.CallSpecial(out float duration))
            onSpecial?.Invoke(duration);
    }
    
    private bool CheckDash()
    {
        if (!isDash)
        {
            if (elapsedTime > dashCoolTime)
            {
                isDash = true;
                elapsedTime = .0f;
            }
        }
        else
        {
            if (elapsedTime > dashDuration)
            {
                isDash = false;
            }
        }
        return isDash;
    }
    
    public override void CallMove(Vector2 direction)
    {
        // if (CheckDash() && direction.sqrMagnitude < PlayerManager.instance.dashSqrDistance)
        // {
        //     Vector2 position = transform.position;
        //     Vector2 endPos;
        //     if (direction.x < 0)
        //     {
        //         endPos = position + direction - paddingPosition;
        //     }
        //     else
        //     {
        //         endPos = position + direction + paddingPosition;
        //     }
        //     CallDashStart(position, endPos, 0.05f);
        // }
        // else
            base.CallMove(direction);
    }
}