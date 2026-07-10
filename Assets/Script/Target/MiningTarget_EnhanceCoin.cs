using UnityEngine;

public class MiningTarget_EnhanceCoin : MiningTarget_Object
{
    private int breakAttackCount;
    private int breakAttackCount_max = 4;
    private int breakAttackCount_min = 2;

    private int index_SE_Damage => 26;
    private int index_SE_Break => 27;

    public virtual void Init()
    {
        breakAttackCount = UnityEngine.Random.Range(breakAttackCount_min, breakAttackCount_max);

        var hp = 100;
        base.Init(hp, 0, 1f);
        Set_BlockMesh();
        base.animScale_rate = 1f;

        Debug.Log("<color=yellow>== enhanceCoin Set ===</color>");
    }

    public override bool Damage(int damage, float _resourceUpRate = 1f)
    {
        var fixDamage = base.hp_max / breakAttackCount;
        return base.Damage(fixDamage);
    }

    public override void BreakFromDamage(float _resourceUpRate = 1f)
    {
        var effect = EffectManager.Inst.Get_Effect(EffectType.BlockBreak);
        effect.transform.position = transform.position + EffectOffset;
        effect.SetActive(true);
        CameraManager.Inst?.ShakeCamera_BlockBreak();

        Debug.Log("<color=yellow>== enhanceCoin BreakFromDamage ===</color>");

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
