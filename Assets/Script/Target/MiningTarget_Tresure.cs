using UnityEngine;

public class MiningTarget_Tresure : MiningTarget_Object
{
    private int treasureValueRate = 10;

    public override void Init(ObjectGenerateParam _objectGenerateParam)
    {
        base.Init(_objectGenerateParam);
    }
    public override void BreakFromDamage()
    {
        // effect
        var effect = EffectManager.Inst.Get_Effect(EffectType.BlockBreak);
        effect.transform.position = transform.position + EffectOffset;
        effect.SetActive(true);

        // ===========
        var getTresureCoin = (int)(BlockGenerateManager.Inst.blockGenerateParam_max.baseValue
                                    * treasureValueRate * objectGenerateParam.so.valueRate);

        InGameManager.Inst.AddGetCoin(getTresureCoin);
        var ui_textCoinGet = UI_PoolManager.Inst.Set_TextCoinGet(transform, Vector3.zero);
        ui_textCoinGet.SetText(StaticManager.Get_BigintegerToString(getTresureCoin), Color.green);
        base.BreakFromDamage();
    }


}
