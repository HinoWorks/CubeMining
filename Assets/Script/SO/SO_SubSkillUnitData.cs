using UnityEngine;
using System;


[System.Serializable]
public class SubSkillUnitData
{
    public int subSkillIndex;
    public string unitName;
    public Sprite icon;
    public GameObject pf;
    public float rate;
    public float interval;
    public float speed;
    public float aliveTime;
    public int count;
    public float size;
}


[CreateAssetMenu(menuName = "SO/SO_SubSkillUnitData")]
public class SO_SubSkillUnitData : ScriptableObject
{
    public SubSkillUnitData[] subSkillUnitDatas;

    public SubSkillUnitData GetSubSkillUnitData(int _subSkillIndex)
    {
        var data = Array.Find(subSkillUnitDatas, data => data.subSkillIndex == _subSkillIndex);
        if (data == null)
        {
            Debug.LogError($"SubSkillUnitData not found: {_subSkillIndex}");
            return null;
        }
        return data;
    }
}
