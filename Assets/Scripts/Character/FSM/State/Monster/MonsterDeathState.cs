using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MonsterDeathState : MonsterBaseState
{
    private float deathTime;
    private float elsapsedTime;
    private bool isDeathCall;
    
    public MonsterDeathState(StateMachine stateMachine, float deathTime) : base(stateMachine)
    {
        this.deathTime = deathTime;
    }
    public override void Enter()
    {
        base.Enter();

        isDeathCall = false;
        elsapsedTime = .0f;
        stateMachine.movement.StopMove();
        StartAnimation(stateMachine.animationData.DeathHash);
    }

    public override void Exit()
    {
        base.Exit();
        
        StopAnimation(stateMachine.animationData.DeathHash);
    }

    public override void Update()
    {
        // base.Update();

        elsapsedTime += Time.deltaTime;

        if (elsapsedTime > deathTime && !isDeathCall)
        {
            stateMachine.controller.CallDeathEnd();
            isDeathCall = true;
        }
    }
}
