using Defines;
using UnityEngine;

namespace Utils
{
    public abstract class CustomText
    {
        private const string gold = "FBAD2F";
        private const string dia = "79EDFF";
        private const string enhanceStone = "466D1D";
        private const string levelGreen = "30FF2F";
        
        public static string SetColor(string data, Color color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{data}</color>";
        }

        public static string SetColor(string data, EColorType colorType)
        {
            switch (colorType)
            {
                case EColorType.Gold:
                    return $"<color=#{gold}>{data}</color>";
                case EColorType.Green:
                    return $"<color=#{levelGreen}>{data}</color>";
            }

            return data;
        }

        public static string SetColor(string data, ECurrencyType type)
        {
            switch (type)
            {
                case ECurrencyType.Gold:
                    return $"<color=#{gold}>{data}</color>";
                case ECurrencyType.Dia:
                    return $"<color=#{dia}>{data}</color>";
                case ECurrencyType.EnhanceStone:
                    return $"<color=#{enhanceStone}>{data}</color>";
                case ECurrencyType.GoldInvitation:
                case ECurrencyType.AwakenInvitation:
                case ECurrencyType.EnhanceInvitation:
                    return $"<color=#{levelGreen}>{data}</color>";
            }

            return data;
        }

        public static string SetSize(string data, int size)
        {
            return $"<size={size}>" + data + "</size>";
        }

        public static Color CustomColor(int r, int g, int b)
        {
            return new Color(r/255f, g/255f, b/255f);
        }
    }
}
