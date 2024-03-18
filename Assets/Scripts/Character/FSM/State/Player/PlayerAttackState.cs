using Defines;
using UnityEngine;
using Utils;

public class PlayerAttackState : PlayerBaseState
{
    protected Vector2 attackDirection;

    public PlayerAttackState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public void SetAttackDirection(Vector2 direction)
    {
        attackDirection = direction;
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.movement.StopMove();
        SetNormalAttackSpeed(stateMachine.status.currentAttackSpeed);
        StartAnimation(stateMachine.animationData.AttackSubstateHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.animationData.AttackSubstateHash);
    }
}

public class PlayerComboAttackState : PlayerAttackState
{
    private int combo = 0;
    private AttackColliderInfo[] attackColliderInfos;
    private float elapsedTime;
    private bool isAttackPerformed;
    private bool isAttackEffectPerformed;
    private bool isShake;

    public PlayerComboAttackState(StateMachine stateMachine, AttackColliderInfo[] info) : base(stateMachine)
    {
        attackColliderInfos = info;
    }

    public override void Enter()
    {
        base.Enter();
        
        elapsedTime = .0f;
        isAttackPerformed = false;
        isAttackEffectPerformed = false;
        isShake = false;
        SetComboAnimation(stateMachine.animationData.ComboHash, combo);
    }

    public override void Exit()
    {
        base.Exit();

        SetComboAnimation(stateMachine.animationData.ComboHash, 0);
    }

    public override void Update()
    {
        // base.Update();
        elapsedTime += Time.deltaTime;

        var data = attackColliderInfos[combo];

        if (combo == 2 && !isAttackEffectPerformed)
        {
            // Debug.Log("Effect");
            stateMachine.attackSystem.AttackEffect(0, attackDirection);
            isAttackEffectPerformed = true;
        }
        // else if (combo == 2 && !isAttackEffectPerformed)
        // {
        //     Debug.Log($"{combo}");
        // }

        if (!isAttackPerformed && elapsedTime > data.startTime / stateMachine.status.currentAttackSpeed)
        {
            // if (combo == 2) Debug.Log("Attack");
            stateMachine.attackSystem.AttackEvent(data.offset, data.type, data.size, combo, data.knockback, data.duration);
            isAttackPerformed = true;
        }
        
        // if (isAttackPerformed && combo == 2 && elapsedTime > CameraController.instance.shakeTime / stateMachine.status.currentAttackSpeed) CameraController.Shake();
        // 흔들림 효과
        if (!isShake && data.shakeTime > 0 && (data.shakeTime / stateMachine.status.currentAttackSpeed) < elapsedTime)
        {
            CameraController.Shake();
            isShake = true;
        }

        if (isAttackPerformed && elapsedTime > data.duration + data.startTime / stateMachine.status.currentAttackSpeed)
        {
            var target = StageManager.instance.TryGetTarget();
            // if (elapsedTime > (attackColliderInfos[combo].duration) / stateMachine.status.currentAttackSpeed)
            if (elapsedTime > (data.duration + data.startTime) / stateMachine.status.currentAttackSpeed)
            {
                // if (combo == 2) Debug.Log("Next");
                if (!ReferenceEquals(target, null))
                {
                    // if (stateMachine.attackSystem.canAttack)
                    if (Vector.BoxDistance(stateMachine.transform.position, target.transform.position, 0.4f, 0.7f) < .3f)
                    {
                        // var enemy = stateMachine.attackSystem.attackRangeSystem.GetEnemyInArea();
                        // if (!ReferenceEquals(enemy, null))
                        {
                            combo = combo + 1 > 2 ? 0 : combo + 1;
                            // stateMachine.controller.CallAttack(enemy.transform.position - stateMachine.transform.position);
                            stateMachine.controller.CallAttack(target.transform.position - stateMachine.transform.position);
                        }
                        // else
                        // {
                        //     combo = 0;
                        //     var targetPos = target.transform.position;
                        //     stateMachine.controller.CallMove(targetPos - stateMachine.transform.position);
                        // }
                    }
                    else
                    {
                        combo = 0;
                        // var targetPos = target.transform.position;
                        // stateMachine.controller.CallMove(targetPos - stateMachine.transform.position);
                        stateMachine.controller.CallMove(target.transform.position - stateMachine.transform.position);
                    }
                }
                else
                {
                    combo = 0;
                    stateMachine.controller.CallIdle();
                }
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void ActOnActiveSkill(Vector3 direction, string skillName)
    {
        base.ActOnActiveSkill(direction, skillName);
        
        combo = 0;
        if (!isAttackPerformed)
            stateMachine.attackSystem.attackEffect[0].SetActive(false);
    }
}

public class PlayerSkillState : PlayerAttackState
{
    public PlayerSkillState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        stateMachine.movement.StopMove();
        SetNormalAttackSpeed(stateMachine.status.currentAttackSpeed);
        StartAnimation(stateMachine.animationData.SkillSubstateHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.animationData.SkillSubstateHash);
    }
}

public class PlayerSkillAttackState : PlayerSkillState
{
    protected float elapsedTime;
    protected AnimSkillData animSkillInfo;
    protected byte shakeIndex;

    public PlayerSkillAttackState(StateMachine stateMachine, AnimSkillData skillData) : base(stateMachine)
    {
        animSkillInfo = skillData;
    }

    public override void Enter()
    {
        base.Enter();

        elapsedTime = .0f;
        shakeIndex = 0;
        stateMachine.movement.StopMove();
        StartAnimation(Animator.StringToHash(animSkillInfo.animParameter));
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(Animator.StringToHash(animSkillInfo.animParameter));
    }

    public override void Update()
    {
        // base.Update();

        elapsedTime += Time.deltaTime;

        if (animSkillInfo.TryGetShakeTime(shakeIndex, out float shakeTime))
        {
            if (shakeTime < elapsedTime)
            {
                CameraController.Shake();
                ++shakeIndex;
            }
        }

        if (elapsedTime > animSkillInfo.skillAnimTime)
        {
            stateMachine.controller.CallIdle();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}