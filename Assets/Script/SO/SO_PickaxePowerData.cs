using UnityEngine;
using System;
using System.Linq;


public enum ParamFormat
{
    Raw = 0,
    Percent = 1,
    Second = 2,
}

[System.Serializable]
public class PickaxePowerParamDisplay
{
    public int index;
    public int order;
    public string paramName;
    public int valueSlot = 1;
    public ParamFormat format = ParamFormat.Raw;
    public string prefix = "";
}


[System.Serializable]
public class PickaxePowerBase
{
    public int index;
    public string skillName;
    public string description;
    public Sprite icon;
    public int unlockLevel;
    public int blockCount;
    public int CD;
    public int maxLevel;
    public GameObject pf;
}


[System.Serializable]
public class PickaxePowerLevel
{
    public int index;
    public int level;

    public float value_1;
    public float value_2;
    public float value_3;
    public float value_4;

    //public int useCount;

    public int req_point;
    public int req_stone;
    public int req_iron;
    public int req_gold;
    public int req_emerald;
    public int req_ruby;
    public int req_sapphire;
    public int req_diamond;

    public float GetValue(int slot)
    {
        return slot switch
        {
            1 => value_1,
            2 => value_2,
            3 => value_3,
            4 => value_4,
            _ => 0f
        };
    }
}



[CreateAssetMenu(menuName = "SO/SO_PickaxePowerData")]
public class SO_PickaxePowerData : ScriptableObject
{
    public PickaxePowerBase[] pickaxePowerBases;
    public PickaxePowerLevel[] pickaxePowerLevels;
    public PickaxePowerParamDisplay[] pickaxePowerParamDisplays;



    public PickaxePowerBase GetPickaxePowerBase(int _index)
    {
        var data = Array.Find(pickaxePowerBases, data => data.index == _index);
        if (data == null)
        {
            //Debug.LogError($"PickaxePowerBase not found: {_index}");
            return null;
        }
        return data;
    }

    public PickaxePowerLevel GetPickaxePowerLevel(int _index, int _level)
    {
        var data = Array.Find(pickaxePowerLevels, data => data.index == _index && data.level == _level);
        if (data == null)
        {
            //Debug.LogError($"PickaxePowerLevel not found: {_index}");
            return null;
        }
        return data;
    }

    public PickaxePowerParamDisplay[] GetParamDisplays(int _index)
    {
        if (pickaxePowerParamDisplays == null || pickaxePowerParamDisplays.Length == 0)
            return Array.Empty<PickaxePowerParamDisplay>();

        return pickaxePowerParamDisplays
            .Where(data => data.index == _index)
            .OrderBy(data => data.order)
            .ToArray();
    }
}
