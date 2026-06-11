using UnityEngine;
using System;



public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
}

public enum ActiveCheckTiming
{
    Passive = 0, // パッシブ効果
    StartIngame = 1, // インゲーム開始時
    Interval_breakBlock_25 = 10, // ブロックを25破壊ごとに
    Interval_5sec = 20, // 5秒間隔
    Interval_attackPickaxe = 30, // ピッケル攻撃時
    Interval_underGround_5 = 40, // 地下5層ごとに
    LastBooster = 50, // 最後のブースター時
}


public enum ArtifactEffectType
{
    None = 0,
    pickaxe_damage = 1,
    pickaxe_attackInterval = 2,
    pickaxe_criticalRate = 3,
    pickaxe_resourceUpRate = 4,
    pickaxe_size = 5,


    all_damage = 10,
    all_attackInterval = 11,

    bomb_damage = 20,
    bomb_size = 21,


    create_bomb = 30,
    create_miniPickaxe = 31,
    create_bonusChest = 32,

    changeBlockRate = 40,
    resourceUpRate = 41,
    blockBreakRate = 42,

    get_ingameTime = 50,
}


[System.Serializable]
public class ArtifactUnitData
{
    public int artifactIndex;
    public string artifactName;
    public string description;
    public Rarity rarity;
    public Sprite icon;
    public ActiveCheckTiming activeCheckTiming;
    public int activeCheckRate;
    public ArtifactEffectType effectType;
    public float value;
    public ArtifactEffectType effectType_2;
    public float value_2;
    public string unit;
}


[System.Serializable]
public class ArtifactGenerateRateData
{
    public int generateLevel;
    public int artifactCount;
    public float baseRate;
    public float deltaInterval;
    public float deltaRate;
}




[CreateAssetMenu(menuName = "SO/SO_ArtifactData")]
public class SO_ArtifactData : ScriptableObject
{
    public ArtifactUnitData[] artifactDatas;
    public ArtifactGenerateRateData[] artifactGenerateRateDatas;


    public ArtifactUnitData Get_ArtifactData(int _artifactIndex)
    {

        var data = Array.Find(artifactDatas, x => x.artifactIndex == _artifactIndex);
        if (data == null)
        {
            //Debug.LogError($"ArtifactUnitData is not found: {_artifactIndex}");
        }
        return data;
    }

    public ArtifactGenerateRateData Get_ArtifactGenerateRateData(int artifactCount)
    {
        var data = Array.Find(artifactGenerateRateDatas, x => x.artifactCount >= artifactCount);
        if (data == null)
        {
            Debug.LogError($"ArtifactGenerateRateData is not found: {artifactCount}");
        }
        return data;
    }
}
