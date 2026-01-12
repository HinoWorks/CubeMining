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
}


[CreateAssetMenu(menuName = "SO/SO_BlockData")]
public class SO_BlockData : ScriptableObject
{
    public BlockData[] blockDatas;


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
