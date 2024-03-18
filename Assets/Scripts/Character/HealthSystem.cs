using System;
using System.Collections;
using System.Data;
using Character.Monster;
using Keiwando.BigInteger;
using UnityEngine;

public class HealthSystem
{
    public BigInteger CurrentMaxHP { get; protected set; }
    public BigInteger CurrentHP { get; protected set; }
    public BigInteger CurrentMaxMP { get; protected set; }
    public BigInteger CurrentMP { get; protected set; }
    public BigInteger CurrentMPRecovery { get; protected set; }
    
    private LayerMask originLayer;
    
    public bool isDead { get; set; } = true;

    private Transform characterPosition;
    public Vector3 damageUIPositionOffset;

    private BaseController controller;
    private BoxCollider2D hitbox;

    private CurrentStatus status;
    private BaseData data;
    
    public void InitHealthSystem(PlayerData data)
    {
        this.data = data;
        this.status = data.status;
        
        hitbox = data.hitBox;
        characterPosition = data.transform;
        controller = data.controller;
        damageUIPositionOffset = Vector3.up * data.damageUIPosition;
        
        CurrentMaxHP = data.status.currentMaxHealth;
        CurrentHP = CurrentMaxHP;
        CurrentMaxMP = data.status.currentMaxMana;
        CurrentMP = CurrentMaxMP;
        CurrentMPRecovery = data.status.currentManaRecovery;

        data.status.onMaxHealthChange += UpdateMaxHealth;
        data.status.onMaxManaChange += UpdateMaxMana;
        data.status.onManaRecoveryChange += UpdateManaRecovery;
    }
    
    public void InitHealthSystem(MonsterData data)
    {
        this.data = data;
        this.status = data.status;
        
        hitbox = data.hitBox;
        characterPosition = data.transform;
        controller = data.controller;
        damageUIPositionOffset = Vector3.up * data.damageUIPosition;

        CurrentMaxHP = data.status.currentMaxHealth;
        CurrentHP = CurrentMaxHP;
    }
    
    private void UpdateMaxHealth(BigInteger value)
    {
        CurrentHP += value - CurrentMaxHP;
        CurrentMaxHP = value;
    }

    private void UpdateMaxMana(BigInteger value)
    {
        CurrentMP += value - CurrentMaxMP;
        CurrentMaxMP = value;
    }

    private void UpdateManaRecovery(BigInteger value)
    {
        CurrentMPRecovery = value;
    }

    public void Resurrection()
    {
        CurrentHP = CurrentMaxHP;
        CurrentMP = CurrentMaxMP;
        controller.CallCurrentHPChange(CurrentHP, CurrentMaxHP);
        controller.CallCurrentMPChange(CurrentMP);
        // Debug.Log($"{CurrentMP}");
        isDead = false;
        // hitbox.enabled = true;
    }

    public void SetHP(BigInteger hp)
    {
        CurrentHP = CurrentMaxHP < hp ? CurrentMaxHP : hp;
    }

    public void SubstractHP(Vector2 direction, AttackData attack)
    {
        BigInteger result = attack.GetDamage(out bool isCrit) * Mathf.FloorToInt(1000+status.currentDamageReduction * 1000) / 1000;
        
        MessageUIManager.instance.ShowDamage(characterPosition.position + damageUIPositionOffset, result, isCrit);
        
        CurrentHP = CurrentHP < result ? 0 : CurrentHP - result;
        controller.CallCurrentHPChange(CurrentHP, CurrentMaxHP);
        controller.CallHit(direction, attack.GetKnockBack());
        if (CurrentHP <= 0)
        {
            isDead = true;
            // hitbox.enabled = false;
            controller.CallDeathStart();
        }
    }

    public void SubstractHP(Vector2 direction, BigInteger value, int knockback)
    {
        // TODO Reduction damage
        BigInteger result = value * Mathf.FloorToInt(1000+status.currentDamageReduction * 1000) / 1000;
        
        MessageUIManager.instance.ShowDamage(characterPosition.position + damageUIPositionOffset, result);
        
        CurrentHP = CurrentHP < result ? 0 : CurrentHP - result;
        controller.CallCurrentHPChange(CurrentHP, CurrentMaxHP);
        controller.CallHit(direction, knockback);
        if (CurrentHP <= 0)
        {
            isDead = true;
            // hitbox.enabled = false;
            controller.CallDeathStart();
        }
    }

    public void HealHP(BigInteger value)
    {
        CurrentHP = CurrentMaxHP < CurrentHP + value ? CurrentMaxHP : CurrentHP + value;
        controller.CallCurrentHPChange(CurrentHP, CurrentMaxHP);
    }

    public bool SubstractMP(BigInteger value)
    {
        if (CurrentMP < value)
            return false;
        CurrentMP -= value;
        controller.CallCurrentMPChange(CurrentMP);
        // Debug.Log($"{CurrentMP}");
        return true;
    }

    public void HealMP(BigInteger value)
    {
        CurrentMP = CurrentMaxMP < CurrentMP + value ? CurrentMaxMP : CurrentMP + value;
        controller.CallCurrentMPChange(CurrentMP);
        // Debug.Log($"{CurrentMP}");
    }
    
    public void StartRecoveryMP()
    {
        manaRecoveryTimer = new WaitForSeconds(1f);
        if (recoveryManaCoroutine != null)
            data.StopCoroutine(RecoveryMana());
        recoveryManaCoroutine = data.StartCoroutine(RecoveryMana());
    }

    private WaitForSeconds manaRecoveryTimer;
    private Coroutine recoveryManaCoroutine;
    private IEnumerator RecoveryMana()
    {
        while (!isDead)
        {
            HealMP(CurrentMPRecovery);
            yield return manaRecoveryTimer;
        }
    }

    public void HealPerKill(int arg1, int arg2)
    {
        HealHP(CurrentMaxHP / 100);
    }
}
