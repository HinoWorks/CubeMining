using UnityEngine;
using System;


[System.Serializable]
public class BlockGenerateParam
{
    public int blockIndex;
    public string unitName;
    public string unitDescription;
    public GameObject pf;
    public GameObject pf_max;
    public int hp;
    public int baseValue;
    public ResourceType resourceType;
}



[CreateAssetMenu(menuName = "SO/SO_BlockGenerateData")]
public class SO_BlockGenerateData : ScriptableObject
{
    public BlockGenerateParam[] blockGenerateParams;


    public BlockGenerateParam GetBlockGenerateParam(int _blockIndex)
    {
        var data = Array.Find(blockGenerateParams, data => data.blockIndex == _blockIndex);
        if (data == null)
        {
            Debug.LogError($"BlockGenerateParam not found: {_blockIndex}");
            return blockGenerateParams[0];
        }
        return data;
    }
}
