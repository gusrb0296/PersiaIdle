using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnState : MonsterBaseState
{
    private float spawnTime;
    private float elapsedTime;
    
    public MonsterSpawnState(StateMachine stateMachine, float spawnTime) : base(stateMachine)
    {
        this.spawnTime = spawnTime;
    }
    
    public override void Enter()
    {
        base.Enter();

        elapsedTime = .0f;
        StartAnimation(stateMachine.animationData.SpawnHash);
        stateMachine.movement.StopMove();
    }

    public override void Exit()
    {
        base.Exit();
        
        StopAnimation(stateMachine.animationData.SpawnHash);
        stateMachine.health.Resurrection();
    }

    public override void Update()
    {
        // base.Update();

        elapsedTime += Time.deltaTime;
        if (elapsedTime > spawnTime)
        {
            stateMachine.controller.CallIdle();
        }
    }
}
