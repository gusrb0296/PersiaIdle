using UnityEngine;

public class MonsterAttackState : MonsterBaseState
{
    public MonsterAttackState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateMachine.movement.StopMove();
        StartAnimation(stateMachine.animationData.AttackSubstateHash);
    }

    public override void Exit()
    {
        base.Exit();
        
        StopAnimation(stateMachine.animationData.AttackSubstateHash);
    }
}

public class MonsterNormalAttackState : MonsterAttackState
{
    protected int combo = 0;
    private AttackColliderInfo[] attackColliderInfos;
    private float elapsedTime;
    private bool isAttackPerformed;
    private bool isAttackEffectPerformed;
    public MonsterNormalAttackState(StateMachine stateMachine, AttackColliderInfo[] info) : base(stateMachine)
    {
        attackColliderInfos = info;
    }

    public override void Enter()
    {
        base.Enter();
        elapsedTime = .0f;
        isAttackPerformed = false;
        isAttackEffectPerformed = false;
        SetNormalAttackSpeed(stateMachine.status.currentAttackSpeed);
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

        if (!isAttackEffectPerformed)
        {
            stateMachine.attackSystem.AttackEffect(0, Vector2.zero);
            isAttackEffectPerformed = true;
        }

        if (!isAttackPerformed && elapsedTime > data.startTime)
        {
            stateMachine.attackSystem.AttackEvent(data.offset, data.type, data.size, combo, data.knockback, data.duration - data.startTime);
            isAttackPerformed = true;
        }
        
        var target = PlayerManager.instance.player;
        if (!ReferenceEquals(target, null))
        {
            if (stateMachine.attackSystem.canAttack)
            {
                var enemy = stateMachine.attackSystem.attackRangeSystem.GetEnemyInArea();
                if (!ReferenceEquals(enemy, null))
                {
                    if (elapsedTime > attackColliderInfos[combo].duration + attackColliderInfos[combo].startTime)
                    {
                        // combo = combo + 1 > 1 ? 0 : combo + 1;
                        // stateMachine.controller.CallAttack(enemy.transform.position - stateMachine.transform.position);
                        stateMachine.controller.CallIdle();
                    }
                }
                else
                {
                    var targetPos = target.transform.position;
                    stateMachine.controller.CallMove(targetPos - stateMachine.transform.position);
                }
            }
            else
            {
                var targetPos = target.transform.position;
                stateMachine.controller.CallMove(targetPos - stateMachine.transform.position);
            }
        }
        else
        {
            stateMachine.controller.CallIdle();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}