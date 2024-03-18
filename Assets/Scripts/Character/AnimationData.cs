using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationData
{
    #region Animation controller parameter name
    
    [Header("Animation Parameter string")]
    [SerializeField] string AttackSubstateParameter = "@Attack";
    [SerializeField] string ComboParameter = "Combo";
    [SerializeField] string DeathParameter  = "Death";
    [SerializeField] string MoveSubstateParameter  = "@Move";
    [SerializeField] string RunParameter  = "Run";
    [SerializeField] string DashParameter  = "Dash";
    [SerializeField] string SpawnParameter  = "Spawn";
    [SerializeField] string HitParameter = "Hit";
    [SerializeField] string IdleParameter = "Idle";
    [SerializeField] string AttackSpeedParameter = "AttackSpeed";
    [SerializeField] string SkillSubstateParameter = "@Skill";

    #endregion

    #region Animation controller parameter Hash

    public int AttackSubstateHash { get; protected set; }
    public int ComboHash { get; protected set; }

    public int DeathHash { get; protected set; }

    public int MoveSubstateHash { get; protected set; }
    public int RunHash { get; protected set; }
    public int DashHash { get; protected set; }

    public int SpawnHash { get; protected set; }
    public int HitHash { get; protected set; }
    public int IdleHash { get; protected set; }
    
    public int AttackSpeedHash { get; protected set; }
    public int SkillSubstateHash { get; protected set; }

    #endregion

    #region Aniamtion Tag Hash
    
    public int DeathTagHash { get; protected set; }
    public int SpawnTagHash { get; protected set; }
    public int[] ComboAtkTagHash { get; protected set; }

    #endregion

    #region Animation Tag String
    [Header("Animation Tag String")]
    [SerializeField] string DeathTag = "Death";
    [SerializeField] string SpawnTag = "Spawn";
    [SerializeField] string Atk1Tag = "Atk1";
    [SerializeField] string Atk2Tag = "Atk2";
    [SerializeField] string Atk3Tag = "Atk3";

    #endregion

    public void InitializeAnimatorHash()
    {
        AttackSubstateHash = Animator.StringToHash(AttackSubstateParameter);
        ComboHash = Animator.StringToHash(ComboParameter);
        DeathHash = Animator.StringToHash(DeathParameter);

        MoveSubstateHash = Animator.StringToHash(MoveSubstateParameter);
        RunHash = Animator.StringToHash(RunParameter);
        DashHash = Animator.StringToHash(DashParameter);

        HitHash = Animator.StringToHash(HitParameter);
        IdleHash = Animator.StringToHash(IdleParameter);

        SpawnHash = Animator.StringToHash(SpawnParameter);

        SkillSubstateHash = Animator.StringToHash(SkillSubstateParameter);
        
        DeathTagHash = Animator.StringToHash(DeathTag);
        SpawnTagHash = Animator.StringToHash(SpawnTag);
        
        ComboAtkTagHash = new int[3];
        ComboAtkTagHash[0] = Animator.StringToHash(Atk1Tag);
        ComboAtkTagHash[1] = Animator.StringToHash(Atk2Tag);
        ComboAtkTagHash[2] = Animator.StringToHash(Atk3Tag);

        AttackSpeedHash = Animator.StringToHash(AttackSpeedParameter);
    }
}