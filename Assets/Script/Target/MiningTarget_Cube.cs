using UnityEngine;

public class MiningTarget_Cube : MiningTargetBase
{
    private Vector3 EffectOffset = new Vector3(0, 0.25f, 0);



    public override bool Damage(int damage)
    {
        return base.Damage(damage);
    }

    public override void BreakFromDamage()
    {
        var effect = EffectManager.Inst.Get_Effect(EffectType.BlockBreak);
        effect.transform.position = transform.position + EffectOffset;
        effect.SetActive(true);

        InGameManager.Inst.AddGetCoin(base.value);
        var ui_textCoinGet = UI_PoolManager.Inst.Set_TextCoinGet(transform, Vector3.zero);
        ui_textCoinGet.SetText(StaticManager.Get_BigintegerToString(base.value));
        base.BreakFromDamage();
    }
}
