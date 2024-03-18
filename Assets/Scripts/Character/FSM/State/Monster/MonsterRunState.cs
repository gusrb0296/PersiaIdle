using System.Collections;
using System.Collections.Generic;
using Defines;
using UnityEngine;

public class MonsterRunState : MonsterMoveState
{
    public MonsterRunState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        StartAnimation(stateMachine.animationData.RunHash);
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(stateMachine.animationData.RunHash);
    }

    public override void Update()
    {
        // base.Update();
        
        if (!ReferenceEquals(PlayerManager.instance.player,null))
        {
            var playerPos = PlayerManager.instance.player.transform.position;
            if (stateMachine.attackSystem.canAttack)
            {
                var enemy = stateMachine.attackSystem.attackRangeSystem.GetEnemyInArea();
                if (!ReferenceEquals(enemy, null))
                    stateMachine.controller.CallAttack(enemy.transform.position - stateMachine.transform.position);
                else
                {
                    stateMachine.controller.CallMove(playerPos - stateMachine.transform.position);
                }
            }
            else
            {
                stateMachine.controller.CallMove(playerPos - stateMachine.transform.position);
            }
        }
        else
        {
            stateMachine.controller.CallIdle();
        }
    }
}