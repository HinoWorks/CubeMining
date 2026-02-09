using UnityEngine;
using System;


[System.Serializable]
public class BlockData
{
    public int blockIndex;
    public string unitName;
    public string unitDescription;
    public Sprite icon;

    public GameObject pf;


    public int hp;
    public int baseValue;
    public float generateInterval;
    public int count;
    public float size;

    public float bigBlockRate;
    public int separateBlock;
}



[System.Serializable]
public class BlockChangeRateData
{
    public int blockIndex;
    public int baseRate;
    public int rate_gold;
    public int rate_iron;
    public int rate_emerald;
    public int rate_4;
    public int rate_5;
    public int rate_6;
}

public enum ResourceType
{
    Stone,
    Gold,
    Iron,
    Emerald,
    Diamond,
    Ruby,
    Sapphire,
}


[CreateAssetMenu(menuName = "SO/SO_BlockData")]
public class SO_BlockData : ScriptableObject
{
    public BlockData[] blockDatas;
    public BlockChangeRateData[] blockChangeRateDatas;



    public BlockData GetBlockData(int _blockIndex)
    {
        var data = Array.Find(blockDatas, data => data.blockIndex == _blockIndex);
        if (data == null)
        {
            Debug.LogError($"BlockData not found: {_blockIndex}");
            return null;
        }
        return data;
    }



}
