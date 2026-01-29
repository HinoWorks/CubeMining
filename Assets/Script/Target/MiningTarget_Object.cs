using UnityEngine;

public class MiningTarget_Object : MiningTargetBase
{
    protected Vector3 EffectOffset = new Vector3(0, 0.25f, 0);
    protected ObjectGenerateParam objectGenerateParam;


    public virtual void Init(ObjectGenerateParam _objectGenerateParam)
    {
        objectGenerateParam = _objectGenerateParam;

        // 生成中のブロックのうち、最大レベルのHPを基準に計算
        //var hp = (int)(BlockGenerateManager.Inst.blockGenerateParam_max.hp * objectGenerateParam.so.hpRate);
        var hp = 10;

        base.Init(hp, 0, objectGenerateParam.so.objectIndex);
        base.animScale_rate = this.transform.localScale.x;
    }
}
