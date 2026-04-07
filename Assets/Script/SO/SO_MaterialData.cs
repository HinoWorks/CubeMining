using UnityEngine;
using System;


[System.Serializable]
public class MaterialData_Layer
{
    public int layer_min;
    public int layer_max;
    public Material[] materials;
}




[CreateAssetMenu(menuName = "SO/SO_MaterialData")]
public class SO_MaterialData : ScriptableObject
{
    public MaterialData_Layer[] materialData_Layers;


    public MaterialData_Layer GetMaterialData_Layer(int _layerIndex)
    {
        var data = Array.Find(materialData_Layers,
        data => data.layer_min <= _layerIndex && data.layer_max > _layerIndex);
        if (data == null)
        {
            Debug.LogError($"MaterialData_Layer not found: {_layerIndex}");
            return null;
        }
        return data;
    }


}
