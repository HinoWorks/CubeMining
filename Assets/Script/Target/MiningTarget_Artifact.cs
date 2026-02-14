using UnityEngine;
using System;
using UnityEngine.Analytics;


public class MiningTarget_Artifact : MiningTargetBase
{
    protected Vector3 EffectOffset = new Vector3(0, 0.25f, 0);
    protected Action breakCallback;
    protected int artifactIndex;
    private int breakAttackCount; // ブロックを破壊するために必要な攻撃回数
    private int breakAttackCount_max = 8;
    private int breakAttackCount_min = 5;

    public override void NotActivate()
    {
        base.NotActivate();
        breakCallback = null;
    }


    public virtual void Init(int _artifactIndex, int _layerIndex)
    {
        artifactIndex = _artifactIndex;
        layerIndex = _layerIndex;

        var hp = 100;
        breakAttackCount = UnityEngine.Random.Range(breakAttackCount_min, breakAttackCount_max);

        base.Init(hp, 0, -1, 0);
        base.animScale_rate = this.transform.localScale.x;

        Debug.Log($"<color=green>== artifact Set: artifactIndex: {artifactIndex} / layerIndex: {layerIndex} ===</color>");
    }

    public override bool Damage(int damage)
    {
        // ダメージを受けた回数で判定する
        var fixDamage = base.hp_max / breakAttackCount;
        Debug.Log($"<color=green>== artifact Damage: damage: {damage} / fixDamage: {fixDamage} => remain hp: {hp - fixDamage} ===</color>");
        return base.Damage(fixDamage);
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
