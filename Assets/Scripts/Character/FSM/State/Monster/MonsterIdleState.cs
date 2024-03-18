using System.Collections;
using System.Collections.Generic;
using Defines;
using UnityEngine;

public class MonsterIdleState : MonsterBaseState
{
    public MonsterIdleState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateMachine.movement.StopMove();
        StartAnimation(stateMachine.animationData.IdleHash);
    }

    public override void Exit()
    {
        base.Exit();
        
        StopAnimation(stateMachine.animationData.IdleHash);
    }

    public override void Update()
    {
        // base.Update();

        if (!ReferenceEquals(PlayerManager.instance.player,null))
        {
            var playerPos = PlayerManager.instance.player.transform.position;
            if (!ReferenceEquals(stateMachine.attackSystem, null))
            {
                if (stateMachine.attackSystem.canAttack)
                {
                    var enemy = stateMachine.attackSystem.attackRangeSystem.GetEnemyInArea();
                    if (!ReferenceEquals(enemy, null))
                        stateMachine.controller.CallAttack(enemy.transform.position - stateMachine.transform.position);
                    else
                    {
                        var targetPos = playerPos;
                        stateMachine.controller.CallMove(targetPos - stateMachine.transform.position);
                    }
                }
                else
                {
                    var targetPos = playerPos;
                    stateMachine.controller.CallMove(targetPos - stateMachine.transform.position);
                }
            }
        }
    }
}