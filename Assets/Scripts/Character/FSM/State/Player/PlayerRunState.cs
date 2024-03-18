using System.Collections;
using System.Collections.Generic;
using Defines;
using UnityEngine;

public class PlayerRunState : PlayerMoveState
{
    public PlayerRunState(StateMachine stateMachine) : base(stateMachine)
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
        
        var target = StageManager.instance.TryGetTarget();
        if (!ReferenceEquals(target, null))
        {
            // if (stateMachine.attackSystem.canAttack)
            if (Utils.Vector.BoxDistance(stateMachine.transform.position, target.transform.position, 0.4f, 0.7f) < .3f)
            {
                stateMachine.controller.CallAttack(target.transform.position - stateMachine.transform.position);
                // var enemy = stateMachine.attackSystem.attackRangeSystem.GetEnemyInArea();
                // if (!ReferenceEquals(enemy, null))
                //     stateMachine.controller.CallAttack(enemy.transform.position - stateMachine.transform.position);
                // else
                // {
                //     var targetPos = target.transform.position;
                //     stateMachine.controller.CallMove(targetPos - stateMachine.transform.position);
                // }
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
