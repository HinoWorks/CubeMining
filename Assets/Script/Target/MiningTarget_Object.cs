using UnityEngine;
using System;
public class MiningTarget_Object : MiningTargetBase
{
    protected Vector3 EffectOffset = new Vector3(0, 0.25f, 0);
    protected ObjectGenerateParam objectGenerateParam;
    protected Action breakCallback;


    public void Set_BreakCallback(Action _callback)
    {
        breakCallback = _callback;
    }
    public override void NotActivate()
    {
        base.NotActivate();
        breakCallback = null;
    }


    public virtual void Init(ObjectGenerateParam _objectGenerateParam, BlockData _blockData, int _layerIndex)
    {
        objectGenerateParam = _objectGenerateParam;
        layerIndex = _layerIndex;

        //base.Init(_blockData.hp, 0, objectGenerateParam.so.objectIndex, 0);
        //base.animScale_rate = this.transform.localScale.x;
    }

    public virtual void Init_MiningTargetBase(int _hp, int _value, int _index, int _layerIndex)
    {
        base.Init(_hp, _value, _index, _layerIndex);
        base.animScale_rate = this.transform.localScale.x;
    }

    public override void BreakFromDamage(float _resourceUpRate = 1f)
    {
        base.BreakFromDamage();
        breakCallback?.Invoke();
    }
}
