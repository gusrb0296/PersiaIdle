using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using Defines;
using UnityEngine;
using Utils;

public class PlayerData : BaseData
{
    public PlayerStatus status { get; protected set; }
    public PlayerController controller { get; protected set; }
    public AttackColliderInfo[] normalAttackColliderInfo;
    public GameObject[] attackEffect;
    // public Vector2[] attackEffectOffset;
    // public Vector3[] attackEffectAngleOffset;
    public AnimSkillData[] skillDatas { get; protected set; }
    public SkillSystem specialSkill; 

    public PlayerData DeployPlayer(PlayerManager manager)
    {
        status = manager.status;
        skillDatas = manager.EquippedSkill;
        InitPlayer();
        CameraController.SetFollow(gameObject);
        // StageManager.instance.SubscribeInStageManager(this);
        // specialSkill.InitSkillSystem(this, SkillManager.instance.playerSpecialActiveSkillData);
        return this;
    }
    
    protected override void Awake()
    {
        base.Awake();
        controller = GetComponent<PlayerController>();
        Debug.Log("Awake player");
    }

    public void InitPlayer()
    {
        Debug.Log("Init player");
        fsm.InitStateMachine(this);
        fsm.TryAddState(EFsmState.Idle, new PlayerIdleState(fsm));
        fsm.TryAddState(EFsmState.Run, new PlayerRunState(fsm));
        fsm.TryAddState(EFsmState.Death, new PlayerDeathState(fsm,deathAnimTime));
        fsm.TryAddState(EFsmState.Spawn, new PlayerSpawnState(fsm, spawnAnimTime));
        // fsm.AddState(EFsmState.Dash, new PlayerDashState(fsm));
        fsm.TryAddState(EFsmState.NormalAttack, new PlayerComboAttackState(fsm, normalAttackColliderInfo));
        // fsm.AddState(EFsmState.SkillAttack1, new PlayerSkill1AttackState(fsm, skillDatas[0]));
        
        movement.InitData(this);
        attackSystem.InitAttackSystem(this);
        spriteController.InitializeController(controller);
        InitializeAnimationLengths();
        health.InitHealthSystem(this);
        
        fsm.TryChangeState(EFsmState.Spawn);
    }

    public void InitHealthSystem()
    {
        // animator.SetBool(animationData.IdleHash, false);
        health.InitHealthSystem(this);
        fsm.TryChangeState(EFsmState.Spawn);
    }

    public void Idle()
    {
        fsm.TryChangeState(EFsmState.Idle);
    }

    public void Wait()
    {
        movement.StopMove();
        fsm.StopStateMachine();
        animator.SetBool(animationData.IdleHash, true);
        // animator.SetBool(animationData.DeathHash, true);
        // health.isDead = true;
        // fsm.ChangeState(EFsmState.Death);
    }

    public void Spawn()
    {
        fsm.TryChangeState(EFsmState.Spawn);
        transform.position = Vector3.zero;
    }


}
