using UnityEngine;

public class MiningTarget_Tresure : MiningTarget_Object
{
    private ResourceType resourceType;
    private int treasureValueRate = 10;
    private int rate_max = 10;
    private int rate_min = 5;

    private int index_SE_Damage => 20;


    public override void Init(ObjectGenerateParam _objectGenerateParam, BlockData _blockData, int _layerIndex)
    {
        var hp = (int)(_blockData.hp * _objectGenerateParam.so.hpRate);
        treasureValueRate = UnityEngine.Random.Range(rate_min, rate_max);
        var getTreasureValue = (int)(_blockData.baseValue * _objectGenerateParam.so.valueRate * treasureValueRate);
        if (getTreasureValue <= 0) getTreasureValue = 1;

        //Debug.Log($"Set_Tresure - hp: {hp}, getTreasureValue: {getTreasureValue}");
        base.Init_MiningTargetBase(hp, getTreasureValue, _objectGenerateParam.so.objectIndex, _layerIndex);

        // ブロックのタイプを自分で抽選
        var blockTypeData = GameParamManager.Get_RandamBlockType(_blockData.blockIndex);
        resourceType = blockTypeData;
        //resourceType = blockTypeData.SelectBlockType();

        Debug.Log($"トレジャーの中身の種類は後で決める！！ resourceType: {resourceType}");

        // TODO here 
    }

    public override void BreakFromDamage()
    {
        // effect
        var effect = EffectManager.Inst.Get_Effect(EffectType.BlockBreak);
        effect.transform.position = transform.position + EffectOffset;
        effect.SetActive(true);
        CameraManager.Inst?.ShakeBlockBreak();

        // ===== treasure value ======
        AddGetResource();

        GameEvent.InGame.PublishGameRecordDataMod_Ingame(GameRecordData_Type.TreasureCount, 1);
        GameEvent.InGame.PublishGameRecordDataMod_Ingame(GameRecordData_Type.Damage, hp_max);

        base.BreakFromDamage();
    }

    private void AddGetResource()
    {
        InGameManager.Inst.AddGetResource(resourceType, base.value);
        for (int i = 0; i < base.value; i++)
        {
            var ui_resourceCont = UI_PoolManager.Inst.Set_GetResourceCont();
            ui_resourceCont.Set_ResourceType(resourceType);
            ui_resourceCont.SetInit(transform.position);
        }

        var getText = UI_PoolManager.Inst.Set_TextCoinGet(transform, Vector3.zero);
        getText.SetText_Coin(StaticManager.Get_BigintegerToString(base.value), SOLoader.UISetting.GetTextColor(resourceType));
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
