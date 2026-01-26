using UnityEngine;
using System;


public enum UnlockCheckType
{
    None,
    GamePlayCount,
    PlayerLevel,
    BlockBreakCount,
    AttackUnitUnlock,
}

public enum UnlockTargetType
{
    None,
    Artifact,

}


[System.Serializable]
public class UnlockData
{
    public int eventIndex;
    public UnlockCheckType unlockCheckType;
    public int checkCount;
    public UnlockTargetType unlockTargetType;

}




[CreateAssetMenu(menuName = "SO/SO_UnlockData")]
public class SO_UnlockData : ScriptableObject
{
    public UnlockData[] unlockDatas;

    public UnlockData Get_UnlockData(int _eventIndex)
    {
        var data = Array.Find(unlockDatas, x => x.eventIndex == _eventIndex);
        if (data == null)
        {
            Debug.LogError($"UnlockData is not found: {_eventIndex}");
        }
        return data;
    }
}
