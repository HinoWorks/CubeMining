using System;
using UnityEngine;


[System.Serializable]
public class IngameStageLevel
{
    public int level;
    /// <summary>このレベルから次へ上がるのに必要な破壊数。</summary>
    public int breakCount;
}


[CreateAssetMenu(menuName = "SO/SO_IngameStageLevelData", fileName = "SO_IngameStageLevelData")]
public class SO_IngameStageLevelData : ScriptableObject
{
    public IngameStageLevel[] stageLevels;
    public int maxLevel => stageLevels != null ? stageLevels.Length : 0;

    public IngameStageLevel GetStageLevel(int level)
    {
        if (stageLevels == null || stageLevels.Length == 0) return null;
        if (level < 1) level = 1;
        if (level > maxLevel) level = maxLevel;
        return Array.Find(stageLevels, x => x.level == level);
    }

    public int GetBreaksRequired(int level)
    {
        var data = GetStageLevel(level);
        return data != null ? data.breakCount : int.MaxValue;
    }
}
