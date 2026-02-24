using UnityEngine;
using System;



public enum ParamCategory
{
    GameSystem,
    Block,
    BlockChangeRate,
    OtherObject,
    Attack
}
public enum ParamType
{
    Unlock,

    // ==== game system param ====
    IngameTime,
    CoinBonusRate,


    // ==== block change rate param ====
    Rate_Gold,
    Rate_Iron,
    Rate_Emerald,
    Rate_Ruby,
    Rate_Sapphire,
    Rate_Diamond,

    // ==== object generate param ====
    Rate_Generate,
    Rate_Value,


    // ==== block param ====
    Value,
    BigBlockRate,
    SeparateBlockCount,



    // ==== attack param ====
    Damage,
    AliveTime,
    CT,
    Count,
    Speed,
    Interval,
    Size,

}


[System.Serializable]
public class SkillTree
{
    public int index;
    public string skillName;
    public string description;
    public Sprite icon;
    public int maxLevel;
    public int baseSkillIndex;
    public ParamCategory paramCategory;
    public int targetIndex;
    public ParamType paramType;
    public float baseValue;
    public float deltaValue;

    public int req_stone;
    public int req_iron;
    public int req_gold;
    public int req_emerald;
    public int req_ruby;
    public int req_sapphire;
    public int req_diamond;
}


[CreateAssetMenu(menuName = "SO/SO_SkillTreeData")]
public class SO_SkillTreeData : ScriptableObject
{
    public SkillTree[] skillTreeDatas;


    public SkillTree GetSkillTreeData(int _skillTreeIndex)
    {
        var data = Array.Find(skillTreeDatas, data => data.index == _skillTreeIndex);
        if (data == null)
        {
            Debug.LogError($"SkillTreeData not found: {_skillTreeIndex}");
            return null;
        }
        return data;
    }


    public SkillTree[] GetSkillTreeDatas(ParamCategory _paramCategory, int _targetIndex)
    {
        var datas = Array.FindAll(skillTreeDatas, data => data.paramCategory == _paramCategory && data.targetIndex == _targetIndex);
        if (datas.Length == 0)
        {
            Debug.LogError($"SkillTreeData not found: {_paramCategory}, {_targetIndex}");
            return null;
        }
        return datas;
    }

    public SkillTree GetSkillTreeDatas(ParamCategory _paramCategory, int _targetIndex, ParamType _paramType)
    {
        var targetDatas = GetSkillTreeDatas(_paramCategory, _targetIndex);
        if (targetDatas == null) return null;
        var data = Array.Find(targetDatas, data => data.paramType == _paramType);
        if (data == null)
        {
            Debug.LogError($"SkillTreeData not found: {_paramCategory}, {_targetIndex}, {_paramType}");
            return null;
        }
        return data;
    }

}
