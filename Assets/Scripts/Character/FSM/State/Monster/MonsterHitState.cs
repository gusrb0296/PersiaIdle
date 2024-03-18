using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState : MonsterBaseState
{
    private float elapsedTime;
    public HitState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        elapsedTime = .0f;
        StartAnimation(stateMachine.animationData.HitHash);
    }

    public override void Exit()
    {
        base.Exit();
        
        StopAnimation(stateMachine.animationData.HitHash);
    }

    public override void Update()
    {
        // base.Update();

        elapsedTime += Time.deltaTime;
        
        if (elapsedTime > 0.2f)
            stateMachine.controller.CallIdle();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    
    
}
