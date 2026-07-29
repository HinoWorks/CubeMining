using UnityEngine;
using System;



public enum ParamCategory
{
    GameSystem,
    Block,
    BlockChangeRate,
    OtherBlock,
    Attack,
    SubSkill
}
public enum ParamType
{
    Unlock,

    // ==== game system param ====
    IngameTime,
    LuckyMineRate,
    LuckyMineRate_Resource,
    BlockRegenRate,
    InstantShatterRate,
    PickaxeAttack_AddIngameTimeRate,
    ResourceBaseUpCount,


    // ==== その他 generate param ====
    Rate_Generate,
    Rate_Value,
    Value,


    // ==== attack param ====
    Damage,
    Count,
    Speed,
    Interval,
    Size,
    CriticalRate,
    ResourceRate,
    AliveTime,
    ExLevel,
    Rate,


    // ==== artifact param ====
    ArtifactSlotCount,


    // ==== block generate param ====
    BlockGenerate_InitialCount,
    BlockGenerate_Count_DeltaTime,
    BlockGenerate_Duration,


    // ==== block tower generate param ====
    TowerGenerate_Unlock,
    TowerGenerate_Count,
    TowerGenerate_Duration,
    TowerGenerate_Height,


    // ==== stage levelup param ====
    StageLevelup_GenerateBlockCount,
    StageLevelup_AddTime,
    StageLevelup_ChangeResourceCount,


    // == end game param ====
    King_Stone,
    King_Game,
    King_Attack
}


[System.Serializable]
public class SkillTreeBase
{
    public int baseIndex;
    public ParamCategory paramCategory;
    public ParamType paramType;
    public int targetIndex;
    public string skillName;
    public string description;
    public Sprite icon;
    public int maxLevel;
    public float deltaValue;
    public float deltaValue2;
}

[System.Serializable]
public class SkillTreeUnit
{
    public int skillTreeIndex;
    public int refIndex;
    public int[] unlockCheckIndexes;
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
    public SkillTreeBase[] skillTreeDatas;
    public SkillTreeUnit[] skillTreeUnits;



    public SkillTreeUnit GetSkillTreeUnitData(int _skillTreeUnitIndex)
    {
        var data = Array.Find(skillTreeUnits, data => data.skillTreeIndex == _skillTreeUnitIndex);
        if (data == null)
        {
            Debug.LogError($"SkillTreeUnit not found: {_skillTreeUnitIndex}");
            return null;
        }
        return data;
    }
    public SkillTreeBase GetSkillTreeBaseData(int _skillTreeIndex)
    {
        var data = Array.Find(skillTreeDatas, data => data.baseIndex == _skillTreeIndex);
        if (data == null)
        {
            Debug.LogError($"SkillTreeData not found: {_skillTreeIndex}");
            return null;
        }
        return data;
    }


    public SkillTreeBase[] GetSkillTreeBaseDatas(ParamCategory _paramCategory, int _targetIndex)
    {
        var datas = Array.FindAll(skillTreeDatas, data => data.paramCategory == _paramCategory && data.targetIndex == _targetIndex);
        if (datas.Length == 0)
        {
            Debug.LogError($"SkillTreeData not found: {_paramCategory}, {_targetIndex}");
            return null;
        }
        return datas;
    }

    public SkillTreeBase GetSkillTreeBaseDatas(ParamCategory _paramCategory, int _targetIndex, ParamType _paramType)
    {
        var targetDatas = GetSkillTreeBaseDatas(_paramCategory, _targetIndex);
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
