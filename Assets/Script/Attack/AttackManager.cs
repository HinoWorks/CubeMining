using UnityEngine;
using System.Collections.Generic;

public class AttackManager : MonoBehaviour
{
    public static AttackManager Inst;
    [SerializeField] List<AttackContBase> attackConts = new List<AttackContBase>();

    private int[] attackUnitIndexes;

    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }

    }



    public void Set_Ready()
    {
        Debug.Log($"TODO == Attack Unit Activate");
        attackUnitIndexes = new int[1] { 0 };
        foreach (var unitIndex in attackUnitIndexes)
        {
            AttackUnitGenerate(unitIndex);
        }
    }

    private void AttackUnitGenerate(int _unitIndex)
    {
        var attackUnitData = SOLoader.AttackUnitData.GetAttackUnitData(_unitIndex);
        var attackUnit = Instantiate(attackUnitData.pf, transform) as GameObject;
        attackUnit.transform.position = transform.position;
        attackUnit.transform.localScale = Vector3.one;

        var attackCont = attackUnit.GetComponent<AttackContBase>();
        attackConts.Add(attackCont);
        attackCont.Init(attackUnitData);
    }


    public void Set_AttackState(bool isStart)
    {
        // 攻撃開始
        foreach (var attackCont in attackConts)
        {
            attackCont.Set_AttackTrigger(isStart);
        }
    }

    public void AttackUnitDelete()
    {
        foreach (var attackCont in attackConts)
        {
            attackCont.OnDestroy();
        }
        attackConts.Clear();
    }


}
