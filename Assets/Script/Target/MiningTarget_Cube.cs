using UnityEngine;
using System;
public class MiningTarget_Cube : MiningTargetBase
{
    private Vector3 EffectOffset = new Vector3(0, 0.25f, 0);
    private BlockSize blockSize;

    private Action breakCallback;

    public override void Init(int _hp, int _value, int _index)
    {
        base.Init(_hp, _value, _index);
        transform.localScale = Vector3.one;
        //hitFlash.Init_Crack();
    }
    public void Set_BreakCallback(Action _callback)
    {
        breakCallback = _callback;
    }
    public void Set_BlockSize(BlockSize _blockSize, float _size)
    {
        blockSize = _blockSize;

        transform.localScale = _size * Vector3.one;
        base.animScale_rate = _size;
    }

    public override void NotActivate()
    {
        base.NotActivate();
        breakCallback = null;
    }

    public override bool Damage(int damage)
    {
        //hitFlash.Flash();
        var isBreak = base.Damage(damage);
        //hitFlash.Set_Crack(hp_rate);
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

        InGameManager.Inst.AddGetCoin(base.value);
        var ui_textCoinGet = UI_PoolManager.Inst.Set_TextCoinGet(transform, Vector3.zero);
        ui_textCoinGet.SetText_Coin(StaticManager.Get_BigintegerToString(base.value), Color.green);
        NotActivate();
    }
}
