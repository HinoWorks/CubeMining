using UnityEngine;
using System;



public enum ParamCategory
{
    GameSystem,
    Block,
    Attack
}
public enum ParamType
{
    Unlock,

    // game system param
    IngameTime,
    BonusRate,



    // block param
    Value,




    // attack param
    Damage,
    AliveTime,
    CT,
    Count,

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
    public int cost;
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

}
