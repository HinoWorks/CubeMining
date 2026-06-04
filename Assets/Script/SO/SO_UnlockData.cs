using UnityEngine;
using System;



public enum UnlockTargetType
{
    None,
    SkillTree,
    Artifact,
    PickaxeCraft,
    PickaxePower
}


[System.Serializable]
public class UnlockData
{
    public int index;
    public int unlockLevel;
    public UnlockTargetType unlockTargetType;
}





[CreateAssetMenu(menuName = "SO/SO_UnlockData")]
public class SO_UnlockData : ScriptableObject
{
    public UnlockData[] unlockDatas;

    public UnlockData Get_UnlockData(int _index)
    {
        var data = Array.Find(unlockDatas, x => x.index == _index);
        if (data == null)
        {
            Debug.LogError($"UnlockData is not found: {_index}");
        }
        return data;
    }


    /// <summary>
    /// 現在のレベルより下の全てのUnlockDataを取得
    /// </summary>
    public UnlockData[] Get_UnlockData_UnderLevel(int _currentLevel)
    {
        var data = Array.FindAll(unlockDatas, x => x.unlockLevel <= _currentLevel);
        return data;
    }

    /// <summary>
    /// 現在のレベルのUnlockDataを取得
    /// </summary>
    public UnlockData Get_UnlockData_NowLevel(int _currentLevel)
    {
        var data = Array.Find(unlockDatas, x => x.unlockLevel == _currentLevel);
        return data;
    }
}
