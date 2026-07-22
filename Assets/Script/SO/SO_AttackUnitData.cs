using UnityEngine;
using System;


/// <summary>
/// ピッケル以外の攻撃Unitデータ
/// </summary>
[System.Serializable]
public class AttackUnitData
{
    public int attackIndex;
    public string unitName;
    public string unitDescription;
    public Sprite icon;
    public GameObject pf;

    public float damageRate;
    public float attackInterval;
    public float criticalRate;

    public float speed;
    public float aliveTime;

    public int count;
    public float size;
}



/// <summary>
/// ピッケルデータ
/// </summary>
[System.Serializable]
public class PickaxeUnitData
{
    public int pickaxeIndex;
    public string pickaxeName;
    public Sprite icon;
    public GameObject pf;
    public int damage;
    public float attackInterval;
    public float criticalRate;
    public float resourceUpRate;
    //public Sprite areaIcon;
    public float size;
    public bool isLast; // 主に最後のピッケル用
}

/// <summary>
/// ピッケル作成に必要なリソースデータ
/// </summary>
[System.Serializable]
public class PickaxeResourceData
{
    public int pickaxeIndex;
    public int req_stone;
    public int req_iron;
    public int req_gold;
    public int req_emerald;
    public int req_ruby;
    public int req_sapphire;
    public int req_diamond;
    public int createTime;
}

public enum PickaxeParamType
{
    Damage,
    AttackInterval,
    CriticalRate,
    ResourceRate,
    AreaSize,
}

[System.Serializable]
public class PickaxeParamBase
{
    public PickaxeParamType paramType;
    public Sprite icon;
    public string paramName;
}




[CreateAssetMenu(menuName = "SO/AttackUnitData")]
public class SO_AttackUnitData : ScriptableObject
{
    public AttackUnitData[] attackUnitDatas;
    public PickaxeUnitData[] pickaxeUnitDatas;
    public PickaxeResourceData[] pickaxeResourceDatas;
    public PickaxeParamBase[] pickaxeParamBases;



    public AttackUnitData GetAttackUnitData(int _attackIndex)
    {
        var data = Array.Find(attackUnitDatas, x => x.attackIndex == _attackIndex);
        if (data == null)
        {
            Debug.LogError($"AttackUnitData not found: {_attackIndex}");
            return null;
        }
        return data;
    }

    public PickaxeUnitData GetPickaxeUnitData(int _pickaxeIndex)
    {
        var data = Array.Find(pickaxeUnitDatas, x => x.pickaxeIndex == _pickaxeIndex);
        if (data == null)
        {
            return null;
        }
        return data;
    }

    public PickaxeResourceData GetPickaxeResourceData(int _pickaxeIndex)
    {
        var data = Array.Find(pickaxeResourceDatas, x => x.pickaxeIndex == _pickaxeIndex);
        if (data == null)
        {
            Debug.LogError($"PickaxeResourceData not found: {_pickaxeIndex}");
            return null;
        }
        return data;
    }

    public PickaxeParamBase GetPickaxeParamBase(PickaxeParamType _paramType)
    {
        var data = Array.Find(pickaxeParamBases, x => x.paramType == _paramType);
        if (data == null)
        {
            Debug.LogError($"PickaxeParamBase not found: {_paramType}");
            return null;
        }
        return data;
    }
}
