using System.Collections;
using System.Collections.Generic;
using Defines;
using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    public PlayerMoveState(StateMachine stateMachine) : base(stateMachine)
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
