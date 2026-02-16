using UnityEngine;

public class MiningTarget_Timer : MiningTarget_Object
{
    private float exTimeBase = 1f;

    public override void Init(ObjectGenerateParam _objectGenerateParam, BlockData _blockData, int _layerIndex)
    {
        base.Init(_objectGenerateParam, _blockData, _layerIndex);
        exTimeBase = _objectGenerateParam.so.valueRate;
        base.Init_MiningTargetBase(hp, 0, _objectGenerateParam.so.objectIndex, _layerIndex);
    }

    public override void BreakFromDamage()
    {
        // effect
        var effect = EffectManager.Inst.Get_Effect(EffectType.BlockBreak);
        effect.transform.position = transform.position + EffectOffset;
        effect.SetActive(true);
        CameraManager.Inst?.ShakeBlockBreak();

        // ===========
        var getExTime = exTimeBase * objectGenerateParam.so.valueRate;

        InGameManager.Inst.AddGetExTime(getExTime);
        var ui_textCoinGet = UI_PoolManager.Inst.Set_TextCoinGet(transform, Vector3.zero);
        ui_textCoinGet.SetText($"+{getExTime.ToString("F1")} Sec", Color.blue);
        base.BreakFromDamage();
    }


}
