using System;
using Defines;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace UI
{
    [Serializable]
    [CreateAssetMenu(fileName = "ShopItemInfo", menuName = "ScriptableObject/Shop")]
    public class ShopItemInfo : ScriptableObject
    {
        public Sprite backEffect;
        public Color backColor;
        public Sprite itemImage;
        public Sprite frontEffect;
        public bool isBest;
    
        public string rewardStr => rewardValue + CustomText.SetSize(CustomText.SetColor(rewardType.ToString(), rewardType), fontSize);
        public bool isBonus;
        public string bonusStr => bonusPercentage.ToString() + "% 보너스";
        public Sprite priceImage;
        public string priceStr => priceValue.ToString();
    
        public int rewardValue;
        public ECurrencyType rewardType;
        public int fontSize;
        public int bonusPercentage;
        public int priceValue;
        public ECurrencyType priceType;
        public string bonusValue => (rewardValue * bonusPercentage / 100) + CustomText.SetSize(CustomText.SetColor(rewardType.ToString(), rewardType), fontSize);
    }
}
