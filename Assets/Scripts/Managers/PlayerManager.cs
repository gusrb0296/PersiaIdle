using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using Keiwando.BigInteger;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public PlayerData playerPrefab;
    public PlayerData player { get; protected set; }

    public BaseLevelSystem levelSystem;
    public PlayerStatus status;

    public string userName;

    public event Action<Equipment, Equipment> onEquipItem;
    public event Action<EEquipmentType> onUnequipItem;
    public event Action<int, AnimSkillData> onEquipSkill;
    public event Action<int> onUnequipSkill;

    public WeaponInfo EquippedWeapon => equipped_Weapon;
    public ArmorInfo EquippedArmor => equipped_Armor;
    public AnimSkillData[] EquippedSkill => equipped_skill;
    public float dashSqrDistance;
    
    WeaponInfo equipped_Weapon = null;
    ArmorInfo equipped_Armor = null;
    AnimSkillData[] equipped_skill;
    [SerializeField] private Sprite playerImage;

    public Dictionary<string, int> skillNamtToSlot;


    public bool CheckStateCanUseSkill()
    {
        if (player.fsm.currentStateType == EFsmState.Stop
            || player.fsm.currentStateType == EFsmState.Death
            || player.fsm.currentStateType == EFsmState.Spawn
            || (int)player.fsm.currentStateType >= (int)EFsmState.SkillAttack1)
            return false;

        var target = StageManager.instance.TryGetTarget();
        if (!ReferenceEquals(target, null))
        {
            if (Utils.Vector.BoxDistance(player.transform.position, target.transform.position, 0.4f, 0.7f) < .5f)
                return true;
        }

        return false;
        // return player.attackSystem.canAttack;
    }

    public Sprite GetUserImage()
    {
        return playerImage;
    }

    private void Awake()
    {
        instance = this;
        equipped_skill = new AnimSkillData[6];
        skillNamtToSlot = new Dictionary<string, int>();
    }

    // 이벤트 설정하는 메서드
    void SetupEventListeners()
    {
        // 레벨업에 대한 스텟 증가를 관찰자로 등록함.
        levelSystem.onLevelChange += status.LevelUpStatusUpgrade;
    }

    public void InitPlayerManager(string nickName)
    {
        userName = nickName;
        levelSystem.LoadLevelExp(userName);
        status.InitStatus();
        status.SetStatusAtLevel(levelSystem.Level);
        SetupEventListeners();

        if (ReferenceEquals(player, null))
        {
            var obj = Instantiate(playerPrefab);
            player = obj.DeployPlayer(this);

            UIManager.instance.TryGetUI<UIPlayerHealthBar>().ShowUI();
            UIManager.instance.TryGetUI<UIPlayerStatusBar>().ShowUI();
            levelSystem.EarnExp(0);
            
            // var ui = UIManager.instance.TryGetUI<UIDeadPanel>();
            // player.controller.onDeathStart += ui.ShowUI;
            levelSystem.onLevelChange += (x)=>
            {
                player.health.HealMP(player.health.CurrentMaxMP);
                player.health.HealHP(player.health.CurrentMaxHP);
            };
        }
        else
        {
            player.InitHealthSystem();
        }
        
        
    }

    public void ApplyOwnedEffect(Equipment equipment)
    {
        // TODO
        Debug.Assert(equipment.type == EEquipmentType.Weapon || equipment.type == EEquipmentType.Armor);
        switch (equipment.type)
        {
            case EEquipmentType.Weapon:
                status.ChangeBaseStat(EStatusType.ATK, equipment.ownedEffect);
                break;
            case EEquipmentType.Armor:
                status.ChangeBaseStat(EStatusType.HP, equipment.ownedEffect);
                break;
        }
    }

    public void EquipItem(Equipment item, bool notify = true)
    {
        // TODO show power
        if (notify)
        {
            var score = new BigInteger(status.BattleScore.ToString());
            EquipItem(item);
            status.InitBattleScore();
            MessageUIManager.instance.ShowPower(status.BattleScore, status.BattleScore - score);
        }
        else
        {
            EquipItem(item);
        }
    }

    private void EquipItem(Equipment equipment)
    {
        //equipment.OnEquipped = true;
        switch (equipment.type)
        {
            case EEquipmentType.Weapon:
            {
                if (ReferenceEquals(equipped_Weapon, null))
                    equipped_Weapon = new WeaponInfo();
                var from = equipped_Weapon;
                UnequippedItem(equipment.type, false);
                equipped_Weapon = equipment as WeaponInfo;
                if (equipped_Weapon == null)
                    break;
                equipped_Weapon.IsEquipped = true;
                status.ChangeBaseStat(EStatusType.ATK, equipped_Weapon.equippedEffect);
                equipped_Weapon.Save(ESaveType.IsEquipped);
                // Debug.Log("장비 장착" + equiped_Weapon.equipName);
                onEquipItem?.Invoke(from, equipment);
                break;
            }
            case EEquipmentType.Armor:
            {
                if (ReferenceEquals(equipped_Armor, null))
                    equipped_Armor = new ArmorInfo();
                var from = equipped_Armor;
                UnequippedItem(equipment.type, false);
                equipped_Armor = equipment as ArmorInfo;
                if (equipped_Armor == null)
                    break;
                equipped_Armor.IsEquipped = true;
                status.ChangeBaseStat(EStatusType.HP, equipped_Armor.equippedEffect);
                equipped_Armor.Save(ESaveType.IsEquipped);
                // Debug.Log("방어구 장착 " + equiped_Armor.equipName);
                onEquipItem?.Invoke(from, equipment);
                break;
            }
        }
    }

    public void UnequippedItem(EEquipmentType type, bool notify = true)
    {
        // TODO show power
        if (notify)
        {
            var score = new BigInteger(status.BattleScore.ToString());
            UnequippedItem(type);
            status.InitBattleScore();
            MessageUIManager.instance.ShowPower(status.BattleScore, status.BattleScore - score);
        }
        else
        {
            UnequippedItem(type);
        }
    }

    private void UnequippedItem(EEquipmentType equipmentType)
    {
        onUnequipItem?.Invoke(equipmentType);
        switch (equipmentType)
        {
            case EEquipmentType.Weapon:
                if (equipped_Weapon == null) return;
                equipped_Weapon.IsEquipped = false;
                status.ChangeBaseStat(EStatusType.ATK, -equipped_Weapon.equippedEffect);
                equipped_Weapon.Save(ESaveType.IsEquipped);
                // Debug.Log("장비 장착 해제" + equiped_Weapon.equipName);
                equipped_Weapon = null;
                break;
            case EEquipmentType.Armor:
                if (equipped_Armor == null) return;
                equipped_Armor.IsEquipped = false;
                status.ChangeBaseStat(EStatusType.HP, -equipped_Armor.equippedEffect);
                equipped_Armor.Save(ESaveType.IsEquipped);
                // Debug.Log("방어구 장착 해제 " + equiped_Armor.equipName);
                equipped_Armor = null;
                break;
        }
    }

    public void EquipSkill(int slot, AnimSkillData skillData)
    {
        onEquipSkill?.Invoke(slot, skillData);
        equipped_skill[slot] = skillData;
        skillData.isEquipped = true;
        player.fsm.TryAddState(skillData.animType, new PlayerSkillAttackState(player.fsm, skillData));
        skillNamtToSlot.Add(skillData.skillName, slot);
    }

    public void UnequipSkill(int slot)
    {
        if (EquippedSkill[slot] == null)
            return;

        onUnequipSkill?.Invoke(slot);

        EquippedSkill[slot].isEquipped = false;
        EquippedSkill[slot].SaveEquip(false);
        player.fsm.TryRemoveState(EquippedSkill[slot].animType);

        var skillname = EquippedSkill[slot].skillName;
        skillNamtToSlot.Remove(skillname);
        equipped_skill[slot] = null;
        
    }

    public bool CanUseSkill(int slot)
    {
        var skill = equipped_skill[slot];
        
        if (skill == null) return false;
        
        if (!CheckStateCanUseSkill()) return false;
        
        return SkillManager.instance.CanUseEquippedSkill(skill.skillName);
    }

    public bool UseSkill(int slot)
    {
        if (CanUseSkill(slot))
        {
            return player.controller.CallSkill(slot);
        }

        return false;
    }

    public bool CanUseSpecial()
    {
        return SkillManager.instance.CanUseSpecial();
    }

    public bool UseSpecialSkill()
    {
        if (CanUseSpecial())
        {
            player.controller.CallSpecialSkill();
            return true;
        }

        return false;
    }

    public void AddBuffToList(TempBuffStatus tempBuff)
    {
        status.AddBuffToList(tempBuff);
    }

    public void RemoveBuffFromList(TempBuffStatus tempBuff)
    {
        status.RemoveBuffFromList(tempBuff);
    }

    public void AddPassiveToList(PassiveStatus passive)
    {
        status.ApplyPassiveStatus(passive);
    }

    public void RemovePassiveToList(PassiveStatus passive)
    {
        status.RemovePassiveStatus(passive);
    }
}