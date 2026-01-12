using UnityEngine;
using System;


[System.Serializable]
public class AttackUnitData
{
    public int attackIndex;
    public string unitName;
    public string unitDescription;
    public Sprite icon;
    public GameObject pf;
    public int damage;
    public float attackInterval;
    public float aliveTime;
    public float ct;
    public int count;
    public float size;
}



[CreateAssetMenu(menuName = "SO/AttackUnitData")]
public class SO_AttackUnitData : ScriptableObject
{
    public AttackUnitData[] attackUnitDatas;


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


}
