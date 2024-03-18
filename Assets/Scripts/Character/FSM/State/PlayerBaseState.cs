using System.Collections;
using System.Collections.Generic;
using Defines;
using UnityEngine;

public class PlayerBaseState : BaseState
{
    protected PlayerController playerController;

    public PlayerBaseState(StateMachine stateMachine) : base(stateMachine)
    {
        playerController = stateMachine.controller as PlayerController;
    }

    public override void Enter()
    {
        base.Enter();
        if (!ReferenceEquals(playerController, null))
        {
            playerController.onDashStart += ActOnDashStart;
            playerController.onActiveSkill += ActOnActiveSkill;
        }
    }

    public override void Exit()
    {
        base.Exit();
        if (!ReferenceEquals(playerController, null))
        {
            playerController.onDashStart -= ActOnDashStart;
            playerController.onActiveSkill -= ActOnActiveSkill;
        }
    }

    public override void ActOnAttack(Vector2 direction)
    {
        stateMachine.TryGetState<PlayerAttackState>(EFsmState.NormalAttack).SetAttackDirection(direction);
        base.ActOnAttack(direction);
    }
}