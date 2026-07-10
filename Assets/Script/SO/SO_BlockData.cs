using UnityEngine;
using System;


[System.Serializable]
public class BlockData
{
    public int blockIndex;
    public string unitName;
    public string unitDescription;
    public BaseBlockType baseBlockType;
    public Sprite icon;
    public GameObject pf;

    // ----
    public int hp;
    public int baseValue;
}


[System.Serializable]
public class BlockChangeRateData
{
    public int blockIndex;
    public int rate_gold;
    public int rate_iron;
    public int rate_emerald;
    public int rate_ruby;
    public int rate_sapphire;
    public int rate_diamond;
}

public enum ResourceType
{
    Stone = 1,
    Iron = 2,
    Gold = 3,
    Emerald = 4,
    Ruby = 5,
    Sapphire = 6,
    Diamond = 7,
}

public enum BaseBlockType
{
    Dirt,
    Dirt_Hard,
    Stone,
    Stone_Hard,
    BlackStone,
    obsidian, // 黒曜石

    // == ??
    Sand,
    Glass,
    Wood,
    Ice,
}


[CreateAssetMenu(menuName = "SO/SO_BlockData")]
public class SO_BlockData : ScriptableObject
{
    public BlockData[] blockDatas;
    public BlockChangeRateData[] blockChangeRateDatas;
    public GameObject pf_Block_ResourceMin;
    public GameObject pf_Block_ResourceMax;
    public GameObject pf_Artifact;
    public GameObject pf_EnhanceCoin;



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

    public BlockChangeRateData GetBlockChangeRateData(int _blockIndex)
    {
        var data = Array.Find(blockChangeRateDatas, data => data.blockIndex == _blockIndex);
        if (data == null)
        {
            Debug.LogError($"BlockChangeRateData not found: {_blockIndex}");
            return null;
        }
        return data;
    }


}
