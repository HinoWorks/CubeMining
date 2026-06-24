using System;
using System.Numerics;
using UnityEngine;

public enum AchievementConditionType
{
    /// <summary>コードから TryUnlock のみ。自動判定しない。</summary>
    Manual = 0,

    /// <summary>GameRecordData の累計値。</summary>
    TotalRecord = 1,

    /// <summary>GameRecordData の1プレイ最高値。</summary>
    OneGameBest = 2,

    /// <summary>PlayerLevelData.level。</summary>
    PlayerLevel = 3,

    /// <summary>所持アーティファクト数（SaveLoader.Get_ArtifactTotalCount）。</summary>
    ArtifactOwned = 4,
    PickaxeOwned = 5,
    SkillTreeOwned = 6,
}

public enum AchievementRecordField
{
    None = 0,
    IngameCount = 1,
    BlockBreakCount = 2,


    OneGame_BlockBreakCount = 20,
    OneGame_TreasureCount = 21,
    OneGame_PlayerExp = 22,
    OneGame_TotalDamage = 23,
    OneGame_MaxDepth = 24,
}

[Serializable]
public class AchievementUnitData
{
    [Tooltip("ゲーム内キー。TryUnlock やログで使用")]
    public string achievementKey = "";

    [Tooltip("Steam パートナーに登録した API Name")]
    public string steamApiName = "";

    public AchievementConditionType conditionType = AchievementConditionType.TotalRecord;
    public AchievementRecordField recordField = AchievementRecordField.IngameCount;

    [Tooltip("達成に必要な値")]
    public int threshold = 1;

    [Tooltip("true の場合、閾値まで SetProgress を送る（進捗型実績向け）")]
    public bool useProgress;
}

[CreateAssetMenu(menuName = "SO/SO_AchievementData")]
public class SO_AchievementData : ScriptableObject
{
    public AchievementUnitData[] achievementDatas;

    public AchievementUnitData GetByKey(string key)
    {
        if (string.IsNullOrEmpty(key) || achievementDatas == null) return null;
        return Array.Find(achievementDatas, x => x.achievementKey == key);
    }

    public AchievementUnitData[] GetAutoEvaluateDatas()
    {
        if (achievementDatas == null) return Array.Empty<AchievementUnitData>();
        return Array.FindAll(
            achievementDatas,
            x => x != null && x.conditionType != AchievementConditionType.Manual);
    }

    public static BigInteger GetTotalRecordValue(GameRecordData data, AchievementRecordField field)
    {
        if (data == null) return BigInteger.Zero;

        return field switch
        {
            AchievementRecordField.IngameCount => data.total_ingameCount,
            AchievementRecordField.BlockBreakCount => data.total_blockBreakCount,
            _ => BigInteger.Zero,
        };
    }

    public static BigInteger GetOneGameBestValue(GameRecordData data, AchievementRecordField field)
    {
        if (data == null) return BigInteger.Zero;

        return field switch
        {
            AchievementRecordField.OneGame_BlockBreakCount => data.oneGame_blockBreakCount,
            AchievementRecordField.OneGame_TreasureCount => data.oneGame_treasureCount,
            AchievementRecordField.OneGame_PlayerExp => data.oneGame_playerExp,
            AchievementRecordField.OneGame_TotalDamage => data.oneGame_totalDamage,
            AchievementRecordField.OneGame_MaxDepth => data.oneGame_maxDepth,
            _ => BigInteger.Zero,
        };
    }
}
