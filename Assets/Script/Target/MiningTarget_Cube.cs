using UnityEngine;
using System;
public class MiningTarget_Cube : MiningTargetBase
{
    [SerializeField] GameObject[] obj_blockMeshes;
    private Vector3 EffectOffset = new Vector3(0, 0.25f, 0);
    private BlockSize blockSize;
    protected ResourceType resourceType;
    private Action breakCallback;

    // -- resource up rate --
    private float resourceUpRate = 1f;


    // -- mesh --
    private float meshThreshold_1 = 0.7f;
    private float meshThreshold_2 = 0.35f;


    // -- se
    private int index_SE_Damage => (int)resourceType;
    private int index_SE_Break => (int)resourceType + 10;


    // -- resource unit --
    private int resourceMax2 = 30;
    private int resourceMax = 10;
    private int resourceMid = 5;


    public override void Init(int _hp, int _value, float _sizeRate)
    {
        base.Init(_hp, _value, _sizeRate);
        //hitFlash.Init_Crack();
        Set_BlockMesh();
    }
    public void Set_BreakCallback(Action _callback)
    {
        breakCallback = _callback;
    }
    public virtual void Set_BlockType(ResourceType _resourceType)
    {
        resourceType = _resourceType;

    }
    /*
    public void Set_BlockSize(BlockSize _blockSize, float _size)
    {
        blockSize = _blockSize;

        transform.localScale = _size * Vector3.one;
        base.animScale_rate = _size;
    }
    */

    private void Set_BlockMesh()
    {
        var currentHpRate = base.hp_rate;
        var targetIndex = 0;
        if (currentHpRate <= meshThreshold_2) targetIndex = 2;
        else if (currentHpRate <= meshThreshold_1) targetIndex = 1;
        for (int i = 0; i < obj_blockMeshes.Length; i++)
        {
            obj_blockMeshes[i].SetActive(i == targetIndex);
        }
    }

    public override void NotActivate()
    {
        base.NotActivate();
        breakCallback = null;
    }

    public override bool Damage(int damage, float _resourceUpRate = 1f)
    {
        //hitFlash.Flash();
        var effect = EffectManager.Inst?.Get_Effect(EffectType.BlockDamage);
        if (effect != null)
        {
            effect.transform.position = transform.position;
            effect.SetActive(true);
        }
        var isBreak = base.Damage(damage, _resourceUpRate);
        //hitFlash.Set_Crack(hp_rate);
        Set_BlockMesh();
        return isBreak;
    }

    protected override void PlaySE_Damage()
    {
        var index_SE = index_SE_Damage;
        if (resourceType != ResourceType.Stone)
        {
            index_SE = 9;
        }
        SoundManager.Inst.PlaySE(index_SE);
    }
    protected override void PlaySE_Break()
    {
        var index_SE = index_SE_Break;
        if (resourceType != ResourceType.Stone)
        {
            index_SE = 19;
        }
        SoundManager.Inst.PlaySE(index_SE);
    }

    public override void BreakFromDamage(float _resourceUpRate = 0f)
    {
        breakCallback?.Invoke();

        var effect = EffectManager.Inst?.Get_Effect(EffectType.BlockBreak);
        if (effect != null)
        {
            effect.transform.position = transform.position + EffectOffset;
            effect.SetActive(true);
        }
        base.WakeUpNeighborBlocks();
        //CameraManager.Inst?.ShakeBlockBreak();
        GameEvent.InGame.PublishGameRecordDataMod_Ingame(GameRecordData_Type.BlockBreakCount, 1);
        GameEvent.InGame.PublishGameRecordDataMod_Ingame(GameRecordData_Type.PlayerExp, 1);
        GameEvent.InGame.PublishGameRecordDataMod_Ingame(GameRecordData_Type.Damage, hp_max);

        AddGetResource(_resourceUpRate);
        NotActivate();
    }

    private void AddGetResource(float _resourceUpRate = 0f)
    {
        var baseResourceValue = base.value + GameParamManager.gameBaseParam.resourceBaseUpCount;
        var getCount = (int)(baseResourceValue
                                    * (1f + _resourceUpRate));
        if (getCount <= 0) getCount = 1;
        InGameManager.Inst.AddGetResource(resourceType, getCount);

        Set_ResourceUnit(getCount);
        //Set_GetText(getCount);
    }


    /// <summary>
    /// リソースを飛ばすエフェクトを設定,　全て飛ばすと重いので刻みでまとめる 
    /// </summary>
    private void Set_ResourceUnit(int _getCount)
    {
        var count_Max2 = _getCount / resourceMax2;
        var remainingCount = _getCount % resourceMax2;

        var count_Max = remainingCount / resourceMax;
        remainingCount = remainingCount % resourceMax;
        var count_Mid = remainingCount / resourceMid;
        remainingCount = remainingCount % resourceMid;

        SetResource(remainingCount, 1, UI_ResourceUnitSize.Min);
        SetResource(count_Mid, resourceMid, UI_ResourceUnitSize.Mid);
        SetResource(count_Max, resourceMax, UI_ResourceUnitSize.Max);
        SetResource(count_Max2, resourceMax2, UI_ResourceUnitSize.Max2);
    }
    private void SetResource(int _repeatCount, int _setCount, UI_ResourceUnitSize _unitSize)
    {
        for (int i = 0; i < _repeatCount; i++)
        {
            var ui_resourceCont = UI_PoolManager.Inst.Set_GetResourceCont();
            ui_resourceCont.Set_ResourceType(resourceType, _setCount, _unitSize);
            ui_resourceCont.SetInit(transform.position);
        }
    }
    private void Set_GetText(int _getCount)
    {
        var getText = UI_PoolManager.Inst.Set_TextCoinGet(transform, Vector3.zero);
        getText.SetText(StaticManager.Get_BigintegerToString(_getCount), SOLoader.UISetting.GetTextColor(resourceType));
    }
}
