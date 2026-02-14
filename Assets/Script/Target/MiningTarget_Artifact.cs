using UnityEngine;
using System;


public class MiningTarget_Artifact : MiningTargetBase
{
    protected Vector3 EffectOffset = new Vector3(0, 0.25f, 0);
    protected Action breakCallback;
    protected int artifactIndex;

    public void Set_BreakCallback(Action _callback)
    {
        breakCallback = _callback;
    }
    public override void NotActivate()
    {
        base.NotActivate();
        breakCallback = null;
    }


    public virtual void Init(int _artifactIndex, int _layerIndex)
    {
        artifactIndex = _artifactIndex;
        layerIndex = _layerIndex;

        // 生成中のブロックのうち、最大レベルのHPを基準に計算
        //var hp = (int)(BlockGenerateManager.Inst.blockGenerateParam_max.hp * objectGenerateParam.so.hpRate);
        var hp = 10;

        base.Init(hp, 0, -1, 0);
        base.animScale_rate = this.transform.localScale.x;
    }

    public override void BreakFromDamage()
    {
        // effect
        var effect = EffectManager.Inst.Get_Effect(EffectType.BlockBreak);
        effect.transform.position = transform.position + EffectOffset;
        effect.SetActive(true);

        InGameManager.Inst.AddGetArtifact(artifactIndex);
        // ===== TODO Here === artifact icon show ======
        //var ui_textArtifactGet = UI_PoolManager.Inst.Set_TextArtifactGet(transform, Vector3.zero);
        //ui_textArtifactGet.SetText_Artifact(artifactValue.ToString(), Color.green);
        base.BreakFromDamage();
        breakCallback?.Invoke();
    }
}
