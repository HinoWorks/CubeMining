using UnityEngine;

public class MiningTarget_Tresure : MiningTarget_Object
{
    private ResourceType resourceType;
    private int treasureValueRate = 5;
    private int rate_max = 7;
    private int rate_min = 4;

    private int index_SE_Damage => 20;
    private int index_SE_Break => 21;


    public override void Init(ObjectGenerateParam _objectGenerateParam, BlockData _blockData)
    {
        base.Init(_objectGenerateParam, _blockData);
        var hp = (int)(_blockData.hp * _objectGenerateParam.so.hpRate);
        treasureValueRate = UnityEngine.Random.Range(rate_min, rate_max);
        var getTreasureValue = (int)(_blockData.baseValue * _objectGenerateParam.so.valueRate * treasureValueRate);
        if (getTreasureValue <= 0) getTreasureValue = 1;

        //Debug.Log($"Set_Tresure - hp: {hp}, getTreasureValue: {getTreasureValue}");
        base.Init_MiningTargetBase(hp, getTreasureValue, _objectGenerateParam.so.objectIndex);
        base.animScale_rate = 1f;

        //現在のブロックタイプの鉱石への変化率から、タイプを設定
        resourceType = GameParamManager.Get_RandamBlockIndex().resourceType;
    }

    public override void BreakFromDamage(float _resourceUpRate = 1f)
    {
        // effect
        var effect = EffectManager.Inst.Get_Effect(EffectType.BlockBreak);
        effect.transform.position = transform.position + EffectOffset;
        effect.SetActive(true);
        CameraManager.Inst?.ShakeCamera_BlockBreak();

        // ===== treasure value ======
        AddGetResource(_resourceUpRate);

        GameEvent.InGame.PublishGameRecordDataMod_Ingame(GameRecordData_Type.TreasureCount, 1);
        GameEvent.InGame.PublishGameRecordDataMod_Ingame(GameRecordData_Type.Damage, hp_max);

        base.BreakFromDamage();
    }

    private void AddGetResource(float _resourceUpRate = 1f)
    {
        var getCount = (int)(base.value * (1f + _resourceUpRate));
        if (getCount <= 0) getCount = 1;
        InGameManager.Inst.AddGetResource(resourceType, getCount);
        for (int i = 0; i < base.value; i++)
        {
            var ui_resourceCont = UI_PoolManager.Inst.Set_GetResourceCont();
            ui_resourceCont.Set_ResourceType(resourceType);
            ui_resourceCont.SetInit(transform.position);
        }

        var getText = UI_PoolManager.Inst.Set_TextCoinGet(transform, Vector3.zero);
        getText.SetText(StaticManager.Get_BigintegerToString(base.value), SOLoader.UISetting.GetTextColor(resourceType));
    }


    protected override void PlaySE_Damage()
    {
        SoundManager.Inst.PlaySE(index_SE_Damage);
    }
    protected override void PlaySE_Break()
    {
        SoundManager.Inst.PlaySE(index_SE_Damage);
    }
}
