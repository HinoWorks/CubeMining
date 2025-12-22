using UnityEngine;
using System;


[System.Serializable]
public class AttackUnitData
{
    public int index;
    public string unitName;
    public string unitDescription;
    public Sprite unitIcon;
    public GameObject pf;
    public int damage_base;
    public float attackDuration;
}



[CreateAssetMenu(menuName = "SO/AttackUnitData")]
public class SO_AttackUnitData : ScriptableObject
{
    public AttackUnitData[] attackUnitDatas;


    public AttackUnitData GetAttackUnitData(int index)
    {
        var data = Array.Find(attackUnitDatas, x => x.index == index);
        if (data == null)
        {
            Debug.LogError($"AttackUnitData not found: {index}");
            return null;
        }
        return data;
    }


}
