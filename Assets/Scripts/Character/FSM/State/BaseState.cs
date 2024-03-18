using System.Collections;
using System.Collections.Generic;
using Defines;
using UnityEngine;

public class BaseState : IState
{
    protected StateMachine stateMachine;

    public BaseState(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public virtual void Enter()
    {
        stateMachine.controller.onAttack += ActOnAttack;
        stateMachine.controller.onIdle += ActOnIdle;
        stateMachine.controller.onMove += ActOnMove;
        stateMachine.controller.onDeathStart += ActOnDeathStart;
    }

    public virtual void Exit()
    {
        stateMachine.controller.onAttack -= ActOnAttack;
        stateMachine.controller.onIdle -= ActOnIdle;
        stateMachine.controller.onMove -= ActOnMove;
        stateMachine.controller.onDeathStart -= ActOnDeathStart;
    }

    public virtual void Update()
    {
    }

    public virtual void PhysicsUpdate()
    {
    }

    public virtual void ActOnIdle()
    {
        stateMachine.TryChangeState(EFsmState.Idle);
    }

    public virtual void ActOnAttack(Vector2 direction)
    {
        stateMachine.TryChangeState(EFsmState.NormalAttack);
    }

    public virtual void ActOnHit(Vector2 direction, float knockBack)
    {
        stateMachine.TryChangeState(EFsmState.Hit);
    }

    public virtual void ActOnDeathStart()
    {
        stateMachine.TryChangeState(EFsmState.Death);
    }

    public virtual void ActOnMove(Vector2 velocity)
    {
        stateMachine.TryChangeState(EFsmState.Run);
    }

    public virtual void ActOnDashStart(Vector2 arg1, Vector2 arg2, float arg3)
    {
        stateMachine.TryChangeState(EFsmState.Dash);
    }

    public virtual void ActOnActiveSkill(Vector3 direction, string skillName)
    {
        stateMachine.TryChangeState(SkillManager.instance.GetSkill(skillName).animType);
    }

    public virtual void StartAnimation(int hash)
    {
        stateMachine.animator.SetBool(hash, true);
    }

    public virtual void SetComboAnimation(int hash, int value)
    {
        stateMachine.animator.SetInteger(hash, value);
    }

    public virtual void StopAnimation(int hash)
    {
        stateMachine.animator.SetBool(hash, false);
    }

    public virtual void SetNormalAttackSpeed(float speed)
    {
        stateMachine.animator.SetFloat(stateMachine.animationData.AttackSpeedHash, speed);
    }
}

public interface IState
{
    public void Enter();
    public void Exit();
    public void Update();
    public void PhysicsUpdate();
}