using UnityEngine;
using System;


public enum AttackType
{
    Always, // 常時発生している
    Shot, // 弾を撃つ、撃ってすぐCTが開始される
    AreaLoop // 範囲がAliveTimeの間続き、 その後CTが発生する
}



[System.Serializable]
public class AttackUnitData
{
    public int attackIndex;
    public string unitName;
    public string unitDescription;
    public Sprite icon;
    public GameObject pf;

    public AttackType attackType;
    public int damage;
    public float attackInterval;

    public float speed;
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
