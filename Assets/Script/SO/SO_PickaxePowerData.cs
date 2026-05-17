using UnityEngine;
using System;




[System.Serializable]
public class PickaxePowerBase
{
    public int index;
    public string skillName;
    public string description;
    public Sprite icon;
}

[System.Serializable]
public class PickaxePowerLevel
{
    public int index;
    public int level;

    public int value_1;
    public int value_2;
    public int value_3;
    public int value_4;

    public int req_point;
    public int req_stone;
    public int req_iron;
    public int req_gold;
    public int req_emerald;
    public int req_ruby;
    public int req_sapphire;
    public int req_diamond;
}



[CreateAssetMenu(menuName = "SO/SO_PickaxePowerData")]
public class SO_PickaxePowerData : ScriptableObject
{
    public PickaxePowerBase[] pickaxePowerBases;
    public PickaxePowerLevel[] pickaxePowerLevels;



    public PickaxePowerBase GetPickaxePowerBase(int _index)
    {
        var data = Array.Find(pickaxePowerBases, data => data.index == _index);
        if (data == null)
        {
            Debug.LogError($"PickaxePowerBase not found: {_index}");
            return null;
        }
        return data;
    }

    public PickaxePowerLevel GetPickaxePowerLevel(int _index)
    {
        var data = Array.Find(pickaxePowerLevels, data => data.index == _index);
        if (data == null)
        {
            Debug.LogError($"PickaxePowerLevel not found: {_index}");
            return null;
        }
        return data;
    }
}
