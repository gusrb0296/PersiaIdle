using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(StateMachine stateMachine) : base(stateMachine)
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
        
        var target = StageManager.instance.TryGetTarget();
        if (!ReferenceEquals(target, null))
        {
            if (Utils.Vector.BoxDistance(stateMachine.transform.position, target.transform.position, 0.4f, 0.7f) < .3f)
            {
                stateMachine.controller.CallAttack(target.transform.position - stateMachine.transform.position);
            }
            else
            {
                var targetPos = target.transform.position;
                stateMachine.controller.CallMove(targetPos - stateMachine.transform.position);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
