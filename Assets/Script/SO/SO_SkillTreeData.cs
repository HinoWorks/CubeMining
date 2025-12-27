using UnityEngine;
using System;



public enum ParamCategory
{
    HP,
    ATK,
    DEF,
    SPD,
    CRIT,
    CRIT_DMG,
}
public enum ParamType
{

}


[System.Serializable]
public class SkillTree
{
    public int index;
    public string skillName;
    public int maxLevel;
    public int unlockCheckIndex;
    public string paramCategory;
    public string paramType;
    public int baseValue;
    public int deltaValue;
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




}
