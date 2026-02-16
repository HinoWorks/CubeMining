using UnityEngine;
using System;
public class MiningTarget_Cube : MiningTargetBase
{
    [SerializeField] GameObject[] obj_blockMeshes;
    [SerializeField] BlockTypeSetter[] blockTypeSetters;
    private Vector3 EffectOffset = new Vector3(0, 0.25f, 0);
    private BlockSize blockSize;
    private BaseBlockType baseBlockType;
    private ResourceType resourceType;

    private Action breakCallback;


    private float meshThreshold_1 = 0.7f;
    private float meshThreshold_2 = 0.35f;


    // -- se
    private int index_SE_Damage => (int)baseBlockType;
    private int index_SE_Break => (int)baseBlockType + 10;


    public override void Init(int _hp, int _value, int _index, int _layerIndex)
    {
        base.Init(_hp, _value, _index, _layerIndex);
        transform.localScale = Vector3.one;
        base.animScale_rate = 1f;
        //hitFlash.Init_Crack();
        Set_BlockMesh();
    }
    public void Set_BreakCallback(Action _callback)
    {
        breakCallback = _callback;
    }
    public void Set_BlockType(BaseBlockType _baseBlockType, ResourceType _resourceType)
    {
        baseBlockType = _baseBlockType;
        resourceType = _resourceType;
        foreach (var blockTypeSetter in blockTypeSetters)
        {
            blockTypeSetter.Set_BlockTypeObject(_resourceType);
        }
    }
    public void Set_BlockSize(BlockSize _blockSize, float _size)
    {
        blockSize = _blockSize;

        transform.localScale = _size * Vector3.one;
        base.animScale_rate = _size;
    }

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

    public override bool Damage(int damage)
    {
        //hitFlash.Flash();
        var effect = EffectManager.Inst.Get_Effect(EffectType.BlockDamage);
        effect.transform.position = transform.position;
        effect.SetActive(true);
        var isBreak = base.Damage(damage);
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

    public override void BreakFromDamage()
    {
        /*
        switch (blockSize)
        {
            case BlockSize.Big:
                BlockGenerateManager.Inst.BreakBigBlock(index, transform.position);
                break;
            case BlockSize.Normal:
                var effect = EffectManager.Inst.Get_Effect(EffectType.BlockBreak);
                effect.transform.position = transform.position + EffectOffset;
                effect.SetActive(true);
                break;
        }
        */

        breakCallback?.Invoke();

        var effect = EffectManager.Inst.Get_Effect(EffectType.BlockBreak);
        effect.transform.position = transform.position + EffectOffset;
        effect.SetActive(true);
        //CameraManager.Inst?.ShakeBlockBreak();
        GameEvent.InGame.PublishGameRecordDataMod_Ingame(GameRecordData_Type.BlockBreakCount, 1);
        GameEvent.InGame.PublishGameRecordDataMod_Ingame(GameRecordData_Type.Damage, hp_max);

        AddGetResource();
        NotActivate();
    }

    private void AddGetResource()
    {
        var resourceRate = resourceType == ResourceType.Stone ? 1f : 0.5f;
        var getCount = (int)resourceRate * base.value;
        if (getCount <= 0) getCount = 1;
        InGameManager.Inst.AddGetResource(resourceType, getCount);

        for (int i = 0; i < getCount; i++)
        {
            var ui_resourceCont = UI_PoolManager.Inst.Set_GetResourceCont();
            ui_resourceCont.Set_ResourceType(resourceType);
            ui_resourceCont.SetInit(transform.position);
        }

        var getText = UI_PoolManager.Inst.Set_TextCoinGet(transform, Vector3.zero);
        getText.SetText_Coin(StaticManager.Get_BigintegerToString(getCount), SOLoader.UISetting.GetTextColor(resourceType));
    }
}
