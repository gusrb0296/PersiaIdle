using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBaseState : BaseState
{
    public MonsterBaseState(StateMachine stateMachine) : base(stateMachine)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.controller.onHit += ActOnHit;
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.controller.onHit -= ActOnHit;
    }
}
