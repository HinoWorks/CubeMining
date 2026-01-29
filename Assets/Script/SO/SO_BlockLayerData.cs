using UnityEngine;
using System;

[System.Serializable]
public class BlockLayerData
{
    public int layerParamIndex;
    public int layerMin;
    public int layerMax;
    public int layerSize;
    public float rate_block1;
    public float rate_block2;
    public float rate_block3;
    public float rate_block4;
    public float rate_block5;
    public float rate_block6;
}



[CreateAssetMenu(menuName = "SO/SO_BlockLayerData")]
public class SO_BlockLayerData : ScriptableObject
{
    public BlockLayerData[] blockLayerDatas;


    public BlockLayerData GetBlockLayerData_ParamPlayer(int _layerParamIndex)
    {
        var data = Array.Find(blockLayerDatas, data => data.layerParamIndex == _layerParamIndex);
        if (data == null)
        {
            Debug.LogError($"BlockLayerData not found: {_layerParamIndex}");
            return null;
        }
        return data;
    }
    public BlockLayerData GetBlockLayerData(int _layerIndex)
    {
        var data = Array.Find(blockLayerDatas, data =>
            data.layerMin <= _layerIndex && data.layerMax > _layerIndex);
        if (data == null)
        {
            Debug.LogError($"BlockLayerData not found: {_layerIndex}");
            return null;
        }
        return data;
    }
}
