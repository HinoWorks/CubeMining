using UnityEngine;
using System;


public enum EventCheckType
{
    None,
    GamePlayCount,
    PlayerLevel,
    BlockBreakCount,
    AttackUnitUnlock,
}

public enum GameEventTargetType
{
    Unlock,


    // アーティファクト生成タイミングは獲得の有無ががユーザーによって分かれるのでここでは制御せず、個別に管理する
    //ゲームプレイ回数毎に確率上昇し、獲得すると0になる
    //内容はランダムに決定される
    //獲得個数に応じて確率上昇カーブを変える
    ArtifactGenerate,

}



[System.Serializable]
public class GameEventUnitData
{
    public int eventIndex;
    public EventCheckType eventCheckType;
    public float checkValue;
    public GameEventTargetType eventType;
    public float value;
}




[CreateAssetMenu(menuName = "SO/SO_GameEventData")]
public class SO_GameEventData : ScriptableObject
{
    public GameEventUnitData[] gameEventDatas;

    public GameEventUnitData Get_GameEventData(int _eventIndex)
    {
        var data = Array.Find(gameEventDatas, x => x.eventIndex == _eventIndex);
        if (data == null)
        {
            Debug.LogError($"GameEventUnitData is not found: {_eventIndex}");
        }
        return data;
    }
}
