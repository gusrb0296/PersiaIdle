using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using Defines;
using Keiwando.BigInteger;
using UnityEngine;
using UnityEngine.Serialization;

public class BaseData : MonoBehaviour
{
    [field: SerializeField] public LayerMask targetLayerMask { get; protected set; }
    public string targetTag;
    [field: SerializeField] public float damageUIPosition { get; protected set; }
    [field: SerializeField] public float deathAnimTime { get; protected set; }
    [field: SerializeField] public float spawnAnimTime { get; protected set; }

    #region Custom scripts
    public StateMachine fsm { get; protected set; }
    public Movement movement { get; protected set; }
    public HealthSystem health { get; protected set; }
    [field: SerializeField] public AnimationData animationData { get; protected set; }
    public AttackSystem attackSystem;
    public SpriteController spriteController { get; protected set; }
    
    #endregion

    #region Unity components

    public Animator animator { get; protected set; }
    public BoxCollider2D hitBox { get; protected set; }

    #endregion
    protected Dictionary<string, float> animationLengths;
    public bool IsDead
    {
        get => health.isDead;
    }

    protected virtual void Awake()
    {
        animationData.InitializeAnimatorHash();
        movement = gameObject.GetComponent<Movement>();
        gameObject.TryGetComponent<AttackSystem>(out attackSystem);
        // attackSystem = gameObject.GetComponent<AttackSystem>();
        fsm = gameObject.GetComponent<StateMachine>();
        spriteController = gameObject.GetComponent<SpriteController>();
        // animator = gameObject.GetComponent<Animator>();

        if (!gameObject.TryGetComponent<Animator>(out Animator anim))
            animator = gameObject.GetComponentInChildren<Animator>();
        else
            animator = anim;
        
        hitBox = gameObject.GetComponent<BoxCollider2D>();
        // hitBox.enabled = false;
        health = new HealthSystem();
    }
    
    protected void InitializeAnimationLengths()
    {
        Debug.Assert(animator != null);
        animationLengths = new Dictionary<string, float>();
        
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            animationLengths[clip.name] = clip.length;
        }
    }

    public float GetAnimationLength(string animationName)
    {
        if (animationLengths.TryGetValue(animationName, out float length))
        {
            return length;
        }
        else
        {
            Debug.LogWarning("Animation not found: " + animationName);
            return 0f;
        }
    }
}