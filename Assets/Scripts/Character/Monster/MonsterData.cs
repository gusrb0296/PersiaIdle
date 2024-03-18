using Defines;
using Keiwando.BigInteger;
using UnityEngine;
using UnityEngine.SearchService;
using UnityEngine.Serialization;

namespace Character.Monster
{
    public class MonsterData : BaseData
    {
        public MonsterStatus status;
        public MonsterController controller { get; protected set; }
        public EMonsterType type;
        public AttackColliderInfo[] attackColliderInfos;

        // public MonsterData DeployMonster(BaseStatus baseStatus, BaseStatus perLevel, int level, RewardData[] rewardDatas)
        // {
        //     status.InitStatus(baseStatus, perLevel, level, rewardDatas);
        //     // TODO
        //     controller.onDeathStart += () => EffectManager.instance.DropReward(transform.position, rewardDatas);
        //     InitMonster();
        //     return this;
        // }
        protected override void Awake()
        {
            base.Awake();
            controller = GetComponent<MonsterController>();

            // controller.onDeathStart += () => hitBox.enabled = false;
        }

        public virtual void InitMonster()
        {
            if (!ReferenceEquals(fsm, null))
            {
                if (type == EMonsterType.Obstacle)
                {
                    fsm.InitStateMachine(this);
                    fsm.TryAddState(EFsmState.Idle, new MonsterIdleState(fsm));
                    fsm.TryAddState(EFsmState.Death, new MonsterDeathState(fsm, deathAnimTime));
                    fsm.TryAddState(EFsmState.Spawn, new MonsterSpawnState(fsm, spawnAnimTime));
                    // fsm.TryAddState(EFsmState.Hit, new MonsterHi)
                }
                else
                {
                    fsm.InitStateMachine(this);
                    fsm.TryAddState(EFsmState.Idle, new MonsterIdleState(fsm));
                    fsm.TryAddState(EFsmState.Run, new MonsterRunState(fsm));
                    fsm.TryAddState(EFsmState.Death, new MonsterDeathState(fsm, deathAnimTime));
                    fsm.TryAddState(EFsmState.Spawn, new MonsterSpawnState(fsm, spawnAnimTime));
                    fsm.TryAddState(EFsmState.NormalAttack, new MonsterNormalAttackState(fsm, attackColliderInfos));    
                }
            }
            
            if (!ReferenceEquals(movement, null))
                movement.InitData(this);
            if (!ReferenceEquals(attackSystem, null))
                attackSystem.InitAttackSystem(this);
            if (!ReferenceEquals(spriteController, null))
                spriteController.InitializeController(controller);
            if (!ReferenceEquals(animator, null))
                InitializeAnimationLengths();
            // health.InitHealthSystem(this);
        }
        
        public void ResurrectMonster()
        {
            fsm.TryChangeState(EFsmState.Spawn);
        }

        public void InitializeData(BaseStatus baseStatus, BaseStatus perLevel, int level, MonsterDropData[] rewardDatas)
        {
            status.InitStatus(baseStatus, perLevel, level, rewardDatas);
            movement.ChangeMaxSpeed(status.currentMovementSpeed);
            health.InitHealthSystem(this);
            ResurrectMonster();
        }

        public void DropReward()
        {
            EffectManager.instance.DropReward(transform.position, status.rewards);
        }

        public void Idle()
        {
            fsm.TryChangeState(EFsmState.Idle);
        }

        public void Wait()
        {
            fsm.currentState.Exit();
            fsm.StopStateMachine();
        }
    }
}