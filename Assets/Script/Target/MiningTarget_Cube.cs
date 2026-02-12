using UnityEngine;
using System;
public class MiningTarget_Cube : MiningTargetBase
{
    [SerializeField] GameObject[] obj_blockMeshes;
    [SerializeField] BlockTypeSetter[] blockTypeSetters;
    private Vector3 EffectOffset = new Vector3(0, 0.25f, 0);
    private BlockSize blockSize;
    private BlockType blockType;

    private Action breakCallback;


    private float meshThreshold_1 = 0.7f;
    private float meshThreshold_2 = 0.35f;

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
    public void Set_BlockType(BlockType _blockType)
    {
        blockType = _blockType;
        foreach (var blockTypeSetter in blockTypeSetters)
        {
            blockTypeSetter.Set_BlockTypeObject(blockType);
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

        AddGetResource();
        NotActivate();
    }

    private void AddGetResource()
    {
        var resourceType = blockType switch
        {
            BlockType.None => ResourceType.Stone,
            BlockType.Gold => ResourceType.Gold,
            BlockType.Iron => ResourceType.Iron,
            BlockType.Emerald => ResourceType.Emerald,
            BlockType.Rate_4 => ResourceType.Diamond,
            BlockType.Rate_5 => ResourceType.Ruby,
            BlockType.Rate_6 => ResourceType.Sapphire,
            _ => ResourceType.Stone,
        };

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
    }
}
