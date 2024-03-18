using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Character;
using Character.Monster;
using UnityEngine;
using Defines;


public class StateMachine : MonoBehaviour
{
    protected Dictionary<EFsmState, IState> states;

    public AnimationData animationData { get; protected set; }
    public Animator animator { get; protected set; }
    public Movement movement { get; protected set; }
    public BaseController controller { get; protected set; }
    public AttackSystem attackSystem { get; protected set; }
    public IState currentState { get; protected set; }
    public EFsmState currentStateType { get; protected set; }
    public HealthSystem health { get; protected set; }
    public SpriteController spriteController { get; protected set; }
    public CurrentStatus status { get; protected set; }

    protected void Init(BaseData data)
    {
        animationData = data.animationData;
        animator = data.animator;
        movement = data.movement;
        attackSystem = data.attackSystem;
        health = data.health;
        spriteController = data.spriteController;
        states = new Dictionary<EFsmState, IState>();
    }

    public void InitStateMachine(PlayerData data)
    {
        Init(data);
        controller = data.controller;
        status = data.status;
    }

    public void InitStateMachine(MonsterData data)
    {
        Init(data);
        controller = data.controller;
        status = data.status;
    }

    public void TryChangeState(EFsmState state)
    {
        if (states.TryGetValue(state, out IState nextState))
        {
            currentState?.Exit();
            currentState = nextState;
            currentStateType = state;
            currentState?.Enter();
        }
    }

    public bool TryAddState(EFsmState type, IState state)
    {
        return states.TryAdd(type, state);
    }

    public bool TryRemoveState(EFsmState type)
    {
        return states.Remove(type);
    }

    protected virtual void Update()
    {
        currentState?.Update();
    }

    protected virtual void FixedUpdate()
    {
        currentState?.PhysicsUpdate();
    }

    public void StopStateMachine()
    {
        currentState?.Exit();
        currentState = null;
        currentStateType = EFsmState.Stop;
    }

    public T TryGetState<T>(EFsmState type) where T : BaseState
    {
        if (states.TryGetValue(type, out var state))
            return state as T;
        return null;
    }
}
