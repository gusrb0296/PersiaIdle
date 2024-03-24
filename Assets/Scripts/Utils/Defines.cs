using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Defines
{
    public enum ELockType
    {
        LockIcon,
        Appear,
    }
    public enum ESkillDataType
    {
        Quantity,
        Level,
        EquipIndex,
        IsOwned,
    }
    public enum ESaveType
    {
        Quantity,
        IsEquipped,
        EnhancementLevel,
        IsOwned,
        EquippedEffect,
        OwnedEffect,
        RequiredEnhanceStone
    }
    public enum EIconType
    {
        Heart,
        Clock,
        Skull
    }
    public enum EDungeonType
    {
        Gold,
        Awaken,
        Enhance,
    }
    public enum ESkillType
    {
        Active,
        Buff,
        Passive,
    }
    public enum ESkillAttackType
    {
        Single,
        Multiple,
    }
    public enum EMonsterType
    {
        Basic, Elite, Boss, Obstacle
    }
    public enum EStageState
    {
        Normal, Inter, Boss,
        Dungeon
    }
    
    public enum EColorType
    {
        Gold,
        Green
    }

    public enum EShopType
    {
        Gold,
        Dia,
        Package,
        DarkMarket,
    }

    public enum EFsmState
    {
        Stop = -1,
        Idle = 0,
        Run,
        Death,
        Spawn,
        Dash,
        Hit,
        NormalAttack,
        SkillAttack1,
        SkillAttack2,
        SkillAttack3,
        SkillAttack4,
        SkillAttack5,
        SkillAttack6,
        SkillAttack7,
        SkillAttack8,
        SkillAttack9,
        SkillAttack10,
        SkillAttack11,
        SkillAttack12,
        SkillAttack13,
        SkillAttack14,
        SkillAttack15,
        SkillAttack16,
        SkillAttack17,
        SkillAttack18,
        SkillAttack19,
        SkillAttack20,
        SkillAttack21,
        SkillAttack22,
        SkillAttack23,
        SkillAttack24,
    }

    public enum EStatusType
    {
        ATK, // 공격
        HP, // 체력
        DMG_REDU, // 데미지 감소
        MP, // 마나
        MP_RECO, // 마나 회복
        CRIT_CH, // 치명타 확률
        CRIT_DMG, // 치명타 증폭
        ATK_SPD, // 공격 속도
        MOV_SPD, // 이동 속도
        SKILL_DMG, // 스킬 증폭
    }

    public enum EAbilityType
    {
        C,
        B,
        A,
        S,
        SS
    }

    public enum ETrainingType
    {
        Normal,
        Awaken,
        Ability,
        Relic,
    }

    public enum ECalculatePositionType
    {
        Circle,
        Line,
        Outback,
        Stop,
    }
    
    public enum EDataType
    {
        Attack, Health, AttackSpeed, Accuracy, CritRange, CritDamage,
        CurrentHealth,
        CurrentExp, MaxExp, CurrentLevel,
    }

    public enum EUpgradeType
    {
        Training,
        Awaken,
        SummonWeapon,
        SummonArmor,
        SummonSkill,
        WeaponEquip,
        ArmorEquip,
        SkillEquip,
        GoldDungeon,
        AwakenDungeon,
        EnhanceDungeon,
    }

    public enum EEquipmentManagerSaveType
    {
        TotalEnhance,
        TotalWeaponComposite,
        TotalArmorComposite,
    }
}

public enum EAchievementType
{
    WeaponEquip = 0,
    ArmorEquip,
    SkillEquip,
    UseSpecialSkill,
    UseSkill,
    
    GoldDungeonLevel = 5,
    AwakenDungeonLevel,
    EnhanceDungeonLevel,
    
    StatUpgradeCount = 10,
    AttackUpgradeCount,
    HealthUpgradeCount,
    
    WeaponSummonCount = 15,
    ArmorSummonCount,
    SkillSummonCount,
    TotalSummonCount,
    
    ClearStageLevel = 20,
    ReachPlayerLevel,
    
    WeaponCompositeCount = 25,
    ArmorCompositeCount,
    
    KillCount = 30,
    
    UseAutoSkill = 35,
    ClickQuestBar,
    
    LightningGem = 40,
    GuardianGem,
    DestinyGem,
    TempestGem,
    RageGem,
    AbyssGem,
    
    EquipEnhanceCount,
    SkillLevelUp,
}

public enum ECurrencyType
{
    Gold=0, // 골드
    Dia, // 다이아
    EnhanceStone, // 강화석
    AwakenStone, // 각성석
    WeaponSummonTicket, // 무기 소환 티켓
    ArmorSummonTicket, // 방어구 소환 티켓
    GoldInvitation, // 골드 던전 입장권
    AwakenInvitation, // 각성 던전 입장권
    EnhanceInvitation,
    Exp,
}

public enum EQuestRewardType
{
    Gold=0,
    Dia,
    EnhanceStone,
    AwakenStone,
    WeaponSummonTicket,
    ArmorSummonTicket,
    GoldInvitation,
    AwakenInvitation,
    EnhanceInvitation,
    Exp,
    BaseAtk,
    BaseHp,
    BaseDef,
    BaseCritCh,
    BaseCritDmg,
    BaseAtkSpd,
}

public enum ENormalRewardType
{
    Gold=0,
    Dia,
    EnhanceStone,
    AwakenStone,
    WeaponSummonTicket,
    ArmorSummonTicket,
    GoldInvitation,
    AwakenInvitation,
    EnhanceInvitation,
    Exp,
    Weapon,
    Armor,
    None
}


// 장비 타입
public enum EEquipmentType
{
    Weapon,
    Armor,
    Skill,
    Accessory
    // 기타 장비 타입...
}

// 희귀도 
public enum ERarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Mythology,
    // Ancient

    None
    // 기타 희귀도...
}