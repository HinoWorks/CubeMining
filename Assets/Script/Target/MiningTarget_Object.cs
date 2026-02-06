using UnityEngine;
using System;
public class MiningTarget_Object : MiningTargetBase
{
    protected Vector3 EffectOffset = new Vector3(0, 0.25f, 0);
    protected ObjectGenerateParam objectGenerateParam;
    private Action breakCallback;


    public void Set_BreakCallback(Action _callback)
    {
        breakCallback = _callback;
    }
    public override void NotActivate()
    {
        base.NotActivate();
        breakCallback = null;
    }


    public virtual void Init(ObjectGenerateParam _objectGenerateParam, int _layerIndex)
    {
        objectGenerateParam = _objectGenerateParam;
        layerIndex = _layerIndex;

        // 生成中のブロックのうち、最大レベルのHPを基準に計算
        //var hp = (int)(BlockGenerateManager.Inst.blockGenerateParam_max.hp * objectGenerateParam.so.hpRate);
        var hp = 10;

        base.Init(hp, 0, objectGenerateParam.so.objectIndex, 0);
        base.animScale_rate = this.transform.localScale.x;
    }
}
