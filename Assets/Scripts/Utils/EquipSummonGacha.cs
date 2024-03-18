using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

// 가챠 확률을 가지고 있기 위한 클래스입니다.
[CreateAssetMenu(fileName = "Gacha", menuName = "ScriptableObject/Summon", order = 0)]
[Serializable]
public class EquipSummonGacha : ScriptableObject
{
    public GachaPerLevel[] eachWeight;
    private int totalWeight;

    public EquipSummonGacha()
    {
        eachWeight = new GachaPerLevel[Enum.GetNames(typeof(ERarity)).Length-1];
        totalWeight = 0;
    }
    
    public virtual string Summon()
    {
        StringBuilder sb = new StringBuilder();
        
        InitWeight();
        
        var ran = Random.Range(1,totalWeight+1);

        GetRarityAndLevel(ref sb, ran);

        return sb.ToString();
    }

    // 장비를 찾는 키로 사용하기 위한 Rarity와 Level을 StringBuilder로 구성하여 넘겨줍니다.
    protected virtual void GetRarityAndLevel(ref StringBuilder sb, int ran)
    {
        int current = 0;
        int rarity = 0;
        int level = 1;
        foreach (var perRarityLevel in eachWeight)
        {
            level = 1;
            for (int i = 0; i < 4; ++i)
            {
                var per = perRarityLevel.GetWeightPerLevel(i);
                Debug.Assert(per != 0, "랜덤값이 오버됩니다.");
                
                current += per;
                if (current >= ran)
                {
                    sb.Append((ERarity)rarity + "_" + level);
                    return;
                }
                level = Mathf.Clamp(level + 1, 1, 4);
            }
            rarity = Mathf.Clamp(rarity + 1, 0, 5);
        }
        // 그럴 일은 없겠지만 끝까지 간 경우에 대한 예외처리
        sb.Append((ERarity)rarity + "_" + level);
        Debug.Assert(false, "확률이 이상합니다.");
    }

    // total weight을 초기화합니다.
    public virtual void InitWeight()
    {
        int ret = 0;
        foreach (var gachaPerRarityLevel in eachWeight)
        {
            ret += gachaPerRarityLevel.GetWeight();
        }

        totalWeight = ret;
    }

    public virtual float GetPercentage(ERarity rarity)
    {
        return eachWeight[(int)rarity].GetPercentage(totalWeight);
    }
#if UNITY_EDITOR
    // Editor에서만 사용하는 메소드로, 세부 확률을 초기화합니다.
    public virtual void InitSubWeight()
    {
        foreach (var gachaPerRarityLevel in eachWeight)
        {
            gachaPerRarityLevel.InitGacha();
        }
    }
#endif
}

// 각 Rarity당 가챠 확률을 구성하는 데이터입니다.
[Serializable]
public class GachaPerLevel
{
    [SerializeField] public int rarityWeight;
    [SerializeField] public int[] subWeight;

    public GachaPerLevel()
    {
        subWeight = new int[4] { 30, 25, 25, 20 };
    }

    // 해당 Rarity에 설정되어 있는 weight를 리턴합니다.
    public int GetWeight()
    {
        int ret = 0;
        foreach (var weight in subWeight)
        {
            ret += rarityWeight * weight;
        }
    
        return ret;
    }

    // 각 레벨에 맞는 확률을 total weight에 맞추어 돌려줍니다.
    public int GetWeightPerLevel(int rareLevel)
    {
        var ret = subWeight[rareLevel] * rarityWeight;
        return ret;
    }

    // 세부 확률을 초기화합니다.
    public void InitGacha()
    {
        subWeight = new int[] { 30, 25, 25, 20 };
    }

    public float GetPercentage(float totalWeight)
    {
        float current = GetWeight();
        return current / totalWeight;
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(EquipSummonGacha))]
public class SummonGachaEditor : Editor
{
    private EquipSummonGacha gacha;

    // 레벨 개수에 맞는 GachaPerRarityLevel 데이터가 없으면 개수에 맞춥니다.
    private void OnEnable()
    {
        gacha = target as EquipSummonGacha;
        int lengthCondition = Enum.GetNames(typeof(ERarity)).Length - 1;

        if (gacha.eachWeight.Length != lengthCondition)
        {
            List<GachaPerLevel> temp = new List<GachaPerLevel>();
            temp.AddRange(gacha.eachWeight);

            if (temp.Count < lengthCondition)
            {
                while (temp.Count < lengthCondition)
                    temp.Add(new GachaPerLevel());
            }
            else if (temp.Count > lengthCondition)
            {
                while (temp.Count > lengthCondition)
                    temp.RemoveAt(temp.Count - 1);
            }

            gacha.eachWeight = temp.ToArray();
            EditorUtility.SetDirty(target);
        }
        
        // obj.InitWeight();
    }

    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();

        // Editor에서만 사용하도록 하기 위한 버튼으로, total weight을 초기화합니다.
        // if (GUILayout.Button("Initialize sum of total weight"))
        // {
        //     obj.InitWeight();
        // }

        // if (GUILayout.Button("Initialize sub weight"))
        // {
        //     obj.InitSubWeight();
        // }

        gacha.InitWeight();
        for (int i = 0; i < Enum.GetNames(typeof(ERarity)).Length-1; ++i)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"{(ERarity)i} 확률");
            GUILayout.Label((100 * gacha.GetPercentage((ERarity)i)).ToString("F5") + "%");
            int total = EditorGUILayout.IntField("Total Weight", gacha.eachWeight[i].rarityWeight);
            if (total != gacha.eachWeight[i].rarityWeight)
            {
                gacha.eachWeight[i].rarityWeight = total;
                EditorUtility.SetDirty(target);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Weight per Level");
            for (int j = 0; j < 4; ++j)
            {
                var input = EditorGUILayout.IntField(gacha.eachWeight[i].subWeight[j]);
                if (input != gacha.eachWeight[i].subWeight[j])
                {
                    gacha.eachWeight[i].subWeight[j] = input;
                    EditorUtility.SetDirty(target);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
    }
}
#endif