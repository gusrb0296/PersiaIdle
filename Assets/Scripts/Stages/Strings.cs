using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strings
{
    public static string[] statusTypeToKor = { "공격력", "체력", "데미지 감소", "마나", "마나 회복", "치명타 확률", "치명타 증폭", "공격 속도", "이동 속도", "스킬 증폭" };
    
    public static string[] skillTypeToKor = { "액티브", "액티브", "패시브" };
    
    public static string[] currencyToKOR = { "골드", "다이아", "강화석", "각성석", "무기 소환권", "방어구 소환권", "골드 던전 입장권" };

    public static string[] dungeonToKOR = { "골드 던전", "각성석 던전", "강화석 던전" };

    public static string exitDungeon = "던전을 나가시겠습니까?";
    
    public static string[] rareKor = { "일반", "레어", "에픽", "유니크", "전설", "신화" };//, "고대" };

    // public static string[] 
    public class StageDifficulties
    {
        public const string EASY = "쉬움";
        public const string NORMAL = "보통";
        public const string HARD = "어려움";
    }

    public class DataType
    {
        const string ATTACK = "공격력";
        const string HEALTH = "체력";
        const string ACCURACY = "명중";
        const string ATTACK_SPEED = "공격속도";
        const string CRIT_RATE = "치명타 확률";
        const string CRIT_DAMAGE = "치명타 피해량";

        static Dictionary<Defines.EDataType, string> dic = new Dictionary<Defines.EDataType, string>()
        {
            { Defines.EDataType.Attack, ATTACK },
            { Defines.EDataType.Health, HEALTH },
            { Defines.EDataType.Accuracy, ACCURACY },
            { Defines.EDataType.AttackSpeed, ATTACK_SPEED },
            { Defines.EDataType.CritRange, CRIT_RATE },
            { Defines.EDataType.CritDamage, CRIT_DAMAGE }
        };

        public static string GetName(Defines.EDataType type)
        {
            return dic[type];
        }
    }

    public class PlayerState
    {
        public const string ANIMATION_SPAWN = "Spawn";
        public const string ANIMATION_IDLE = "Idle";
        public const string ANIMATION_RUN = "Run";
        public const string ANIMATION_MELEEATTACK = "MeleeAttack";
        public const string ANIMATION_RANGEDATTACK = "RangedAttack";
        public const string ANIMATION_MELEEATTACK_TRIGGER = "MeleeAttackTrigger";
        public const string ANIMATION_DEATH = "Death";
    }

    public class CharacterTag
    {
        public const string TAG_PLAYER = "Player";
        public const string TAG_MONSTER = "Monster";
    }

    public class EquipmentLevelUp
    {
        public const string TITLE_LEVELUP = "장비 레벨 업";
        public const string TITLE_TRANSCENDENCE = "장비 초월 강화";
         
        public const string BUTTON_LEVELUP = "레벨 업";
        public const string BUTTON_TRANSCENDENCE = "초월";

    }
}
