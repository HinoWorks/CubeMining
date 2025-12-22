using UnityEngine;

public class MiningTarget_Cube : MiningTargetBase
{




    public override bool Damage(int damage)
    {
        return base.Damage(damage);
    }

    public override void BreakFromDamage()
    {
        InGameManager.Inst.AddGetCoin(base.value);
        var ui_textCoinGet = UI_PoolManager.Inst.Set_TextCoinGet(transform, Vector3.zero);
        ui_textCoinGet.SetText(StaticManager.Get_BigintegerToString(base.value));
        base.BreakFromDamage();
    }
}
