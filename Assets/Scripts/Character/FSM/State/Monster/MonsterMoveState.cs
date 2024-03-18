using System.Collections;
using System.Collections.Generic;
using Defines;
using UnityEngine;

public class MonsterMoveState : MonsterBaseState
{
    public MonsterMoveState(StateMachine stateMachine) : base(stateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
        
        StartAnimation(stateMachine.animationData.MoveSubstateHash);
    }

    public override void Exit()
    {
        base.Exit();
        
        StopAnimation(stateMachine.animationData.MoveSubstateHash);
    }
}
