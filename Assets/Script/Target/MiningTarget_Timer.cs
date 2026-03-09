using UnityEngine;

public class MiningTarget_Timer : MiningTarget_Object
{
    private float exTimeBase = 1f;
    private int index_SE_Damage => 22;
    private int index_SE_Break => 23;

    public override void Init(ObjectGenerateParam _objectGenerateParam, BlockData _blockData, int _layerIndex)
    {
        base.Init(_objectGenerateParam, _blockData, _layerIndex);
        exTimeBase = _objectGenerateParam.so.valueRate;
        base.Init_MiningTargetBase(hp, 0, _objectGenerateParam.so.objectIndex, _layerIndex);
    }


    protected override void PlaySE_Damage()
    {
        SoundManager.Inst.PlaySE(index_SE_Damage);
    }
    protected override void PlaySE_Break()
    {
        SoundManager.Inst.PlaySE(index_SE_Break);
    }

    public override void BreakFromDamage(float _resourceUpRate = 1f)
    {
        // effect
        var effect = EffectManager.Inst.Get_Effect(EffectType.BlockBreak);
        effect.transform.position = transform.position + EffectOffset;
        effect.SetActive(true);
        CameraManager.Inst?.ShakeBlockBreak();

        // ===========
        var getExTime = exTimeBase + objectGenerateParam.valueRate_total;

        InGameManager.Inst.AddGetExTime(getExTime);
        var ui_textCoinGet = UI_PoolManager.Inst.Set_TextCoinGet(transform, Vector3.zero);
        ui_textCoinGet.SetText($"+{getExTime.ToString("F1")} <size=75%>Sec</size>");

        var ui_timeText = UI_PoolManager.Inst.Set_TimeText();
        ui_timeText.SetText($"+{getExTime.ToString("F1")} <size=75%>Sec</size>");
        base.BreakFromDamage();
    }


}
