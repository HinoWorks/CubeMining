using UnityEngine;
using System;
using UnityEngine.Analytics;


public class MiningTarget_Artifact : MiningTarget_Object
{

    protected int artifactIndex;
    private int breakAttackCount; // ブロックを破壊するために必要な攻撃回数
    private int breakAttackCount_max = 8;
    private int breakAttackCount_min = 5;

    private int index_SE_Damage => 26;
    private int index_SE_Break => 27;

    public virtual void Init(int _artifactIndex, int _layerIndex)
    {
        artifactIndex = _artifactIndex;
        layerIndex = _layerIndex;

        var hp = 100;
        breakAttackCount = UnityEngine.Random.Range(breakAttackCount_min, breakAttackCount_max);

        base.Init(hp, 0, -1, 0);
        Set_BlockMesh();
        base.animScale_rate = this.transform.localScale.x;

        Debug.Log($"<color=green>== artifact Set: artifactIndex: {artifactIndex} / layerIndex: {layerIndex} ===</color>");
    }

    public override bool Damage(int damage, float _resourceUpRate = 1f)
    {
        // ダメージを受けた回数で判定する
        var fixDamage = base.hp_max / breakAttackCount;
        Debug.Log($"<color=green>== artifact Damage: damage: {damage} / fixDamage: {fixDamage} => remain hp: {hp - fixDamage} ===</color>");

        return base.Damage(fixDamage);
    }

    public override void BreakFromDamage(float _resourceUpRate = 1f)
    {
        // effect
        var effect = EffectManager.Inst.Get_Effect(EffectType.BlockBreak);
        effect.transform.position = transform.position + EffectOffset;
        effect.SetActive(true);
        CameraManager.Inst?.ShakeBlockBreak();

        InGameManager.Inst.AddGetArtifact(artifactIndex);

        // ===== TODO Here === artifact icon show ======
        var ui_textArtifactGet = UI_PoolManager.Inst.Set_GetArtifactCont();
        ui_textArtifactGet.SetInit(artifactIndex, transform.position);


        base.BreakFromDamage();
        breakCallback?.Invoke();
    }


    protected override void PlaySE_Damage()
    {
        SoundManager.Inst.PlaySE(index_SE_Damage);
    }

    protected override void PlaySE_Break()
    {
        SoundManager.Inst.PlaySE(index_SE_Break);
    }
}
