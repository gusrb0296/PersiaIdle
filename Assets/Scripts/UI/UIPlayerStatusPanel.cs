using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerStatusPanel : UIPanel
{
    [SerializeField] private TMP_Text userName;
    [SerializeField] private TMP_Text level;
    [SerializeField] private TMP_Text attack;
    [SerializeField] private TMP_Text maxHealth;
    [SerializeField] private TMP_Text damageReduction;
    [SerializeField] private TMP_Text mana;
    [SerializeField] private TMP_Text manaRecovery;
    [SerializeField] private TMP_Text criticalChance;
    [SerializeField] private TMP_Text criticalDamage;
    [SerializeField] private TMP_Text attackSpeed;
    [SerializeField] private TMP_Text movementSpeed;
    [SerializeField] private TMP_Text skillDamage;
    [SerializeField] private TMP_Text battleScore;

    public override void ShowUI()
    {
        base.ShowUI();

        DisplayPlayerStatus();
        SubscribeEvents();
    }
    public override void CloseUI()
    {
        base.CloseUI();
        UnsubscribeEvents();
    }
    
    private void DisplayPlayerStatus()
    {
        DisplayNameUpdate(PlayerManager.instance.userName);
        DisplayLevelUpdate(PlayerManager.instance.levelSystem.Level);
        DisplayAttackUpdate(PlayerManager.instance.status.currentAttack);
        DisplayHealthUpdate(PlayerManager.instance.status.currentMaxHealth);
        DisplayDamageReductionUpdate(PlayerManager.instance.status.currentDamageReduction);
        DisplayManaUpdate(PlayerManager.instance.status.currentMaxMana);
        DisplayManaRecoveryUpdate(PlayerManager.instance.status.currentManaRecovery);
        DisplayCriticalChanceUpdate(PlayerManager.instance.status.currentCriticalChance);
        DisplayCriticalDamageUpdate(PlayerManager.instance.status.currentCriticalDamage);
        DisplayAttackSpeedUpdate(PlayerManager.instance.status.currentAttackSpeed);
        DisplayMovementSpeedUpdate(PlayerManager.instance.status.currentMovementSpeed);
        DisplaySkillDamageUpdate(PlayerManager.instance.status.currentSkillDamage);
        DisplayBattleScore(PlayerManager.instance.status.BattleScore);
    }
    private void SubscribeEvents()
    {
        PlayerManager.instance.levelSystem.onLevelChange += DisplayLevelUpdate;
        PlayerManager.instance.status.onAttackChange += DisplayAttackUpdate;
        PlayerManager.instance.status.onMaxHealthChange += DisplayHealthUpdate;
        PlayerManager.instance.status.onDamageReductionChange += DisplayDamageReductionUpdate;
        PlayerManager.instance.status.onMaxManaChange += DisplayManaUpdate;
        PlayerManager.instance.status.onManaRecoveryChange += DisplayManaRecoveryUpdate;
        PlayerManager.instance.status.onCriticalChanceChange += DisplayCriticalChanceUpdate;
        PlayerManager.instance.status.onCriticalDamageChange += DisplayCriticalDamageUpdate;
        PlayerManager.instance.status.onAttackSpeedChange += DisplayAttackSpeedUpdate;
        PlayerManager.instance.status.onMovementSpeedChange += DisplayMovementSpeedUpdate;
        PlayerManager.instance.status.onSkillDamageChange += DisplaySkillDamageUpdate;
        PlayerManager.instance.status.onBattleScoreChange += DisplayBattleScore;
    }
    private void UnsubscribeEvents()
    {
        PlayerManager.instance.levelSystem.onLevelChange -= DisplayLevelUpdate;
        PlayerManager.instance.status.onAttackChange -= DisplayAttackUpdate;
        PlayerManager.instance.status.onMaxHealthChange -= DisplayHealthUpdate;
        PlayerManager.instance.status.onDamageReductionChange -= DisplayDamageReductionUpdate;
        PlayerManager.instance.status.onMaxManaChange -= DisplayManaUpdate;
        PlayerManager.instance.status.onManaRecoveryChange -= DisplayManaRecoveryUpdate;
        PlayerManager.instance.status.onCriticalChanceChange -= DisplayCriticalChanceUpdate;
        PlayerManager.instance.status.onCriticalDamageChange -= DisplayCriticalDamageUpdate;
        PlayerManager.instance.status.onAttackSpeedChange -= DisplayAttackSpeedUpdate;
        PlayerManager.instance.status.onMovementSpeedChange -= DisplayMovementSpeedUpdate;
        PlayerManager.instance.status.onSkillDamageChange -= DisplaySkillDamageUpdate;
        PlayerManager.instance.status.onBattleScoreChange -= DisplayBattleScore;
    }
    public void DisplayNameUpdate(string nickname) { userName.text = nickname; }
    public void DisplayLevelUpdate(int currentLv) { level.text = currentLv.ToString(); }
    public void DisplayAttackUpdate(BigInteger value) { attack.text = value.ChangeToShort(); }
    public void DisplayHealthUpdate(BigInteger value) { maxHealth.text = value.ChangeToShort(); }
    public void DisplayDamageReductionUpdate(float value) { damageReduction.text = $"{Mathf.FloorToInt(value * 100).ToString()}%"; }
    public void DisplayManaUpdate(BigInteger value) { mana.text = value.ChangeToShort(); }
    public void DisplayManaRecoveryUpdate(BigInteger value) { manaRecovery.text = value.ChangeToShort(); }
    public void DisplayCriticalChanceUpdate(float value) { criticalChance.text = $"{Mathf.FloorToInt(value * 100).ToString()}%"; }
    public void DisplayCriticalDamageUpdate(BigInteger value) { criticalDamage.text = $"{value.ChangeToShort()}%"; }
    public void DisplayAttackSpeedUpdate(float value) { attackSpeed.text = $"{Mathf.FloorToInt(value * 100).ToString()}%"; }
    public void DisplayMovementSpeedUpdate(float value) { movementSpeed.text = $"{Mathf.FloorToInt(value * 100).ToString()}%"; }
    public void DisplaySkillDamageUpdate(BigInteger value) { skillDamage.text = $"{value.ChangeToShort()}%"; }

    public void DisplayBattleScore(BigInteger value) { battleScore.text = $"{value.ChangeToShort()}"; }
}