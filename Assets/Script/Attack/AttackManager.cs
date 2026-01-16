using UnityEngine;
using System.Collections.Generic;

public class AttackManager : MonoBehaviour
{
    public static AttackManager Inst;
    [SerializeField] List<AttackContBase> attackConts = new List<AttackContBase>();
    private bool isAttacking = false;

    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }

    }



    public void Set_Ready()
    {
        Debug.Log($"TODO == Attack Unit Activate");
        isAttacking = false;
        foreach (var attackParam in GameParamManager.list_attackParam)
        {
            if (!attackParam.isActive) continue;
            AttackUnitGenerate(attackParam);
        }
    }

    private void AttackUnitGenerate(AttackParam _attackParam)
    {
        var attackUnit = Instantiate(_attackParam.so.pf, transform) as GameObject;
        attackUnit.transform.position = transform.position;
        attackUnit.transform.localScale = Vector3.one;

        var attackCont = attackUnit.GetComponent<AttackContBase>();
        attackConts.Add(attackCont);
        attackCont.Init(_attackParam);
    }


    public void Set_AttackState(bool isStart)
    {
        // 攻撃開始
        isAttacking = isStart;
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
