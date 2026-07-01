using UnityEngine;
using System.Collections.Generic;

public class SubSkillManager : MonoBehaviour
{
    public static SubSkillManager Inst;
    [SerializeField] List<SubSkillCont_Base> subSkillConts = new List<SubSkillCont_Base>();



    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }
    }


    public void Set_Ready()
    {
        // 攻撃ユニット生成 == スキルツリー分のパラメータを読み込む
        foreach (var subSkillParam in GameParamManager.list_subSkillParam)
        {
            //Debug.Log("<color=purple>DEBUG ---- サブスキル全開放 ----</color>");
            if (!subSkillParam.isActive) continue;
            SubSkillUnitGenerate(subSkillParam);
        }
    }

    private void SubSkillUnitGenerate(SubSkillParam _subSkillParam)
    {
        var subSkillUnit = Instantiate(_subSkillParam.so.pf, transform) as GameObject;
        subSkillUnit.transform.position = transform.position;
        subSkillUnit.transform.localScale = Vector3.one;

        var subSkillCont = subSkillUnit.GetComponent<SubSkillCont_Base>();
        subSkillConts.Add(subSkillCont);
        subSkillCont.Init(_subSkillParam);
    }



    public void Set_SubSkillState(bool isStart)
    {
        // 攻撃開始
        foreach (var subSkillCont in subSkillConts)
        {
            subSkillCont.Set_AttackTrigger(isStart);
        }
    }


    public void SubSkillUnitDeleteAll()
    {
        foreach (var subSkillCont in subSkillConts)
        {
            subSkillCont.OnDestroy();
        }
        subSkillConts.Clear();
    }

}
