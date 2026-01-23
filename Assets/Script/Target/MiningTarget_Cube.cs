using UnityEngine;

public class MiningTarget_Cube : MiningTargetBase
{
    //[SerializeField] HitFlash hitFlash;
    private Vector3 EffectOffset = new Vector3(0, 0.25f, 0);
    private BlockSize blockSize;



    public override void Init(int _hp, int _value, int _index)
    {
        base.Init(_hp, _value, _index);
        //hitFlash.Init_Crack();
    }
    public void Set_BlockSize(BlockSize _blockSize, float _size)
    {
        blockSize = _blockSize;

        transform.localScale = _size * Vector3.one;
        base.animScale_rate = _size;
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

        InGameManager.Inst.AddGetCoin(base.value);
        var ui_textCoinGet = UI_PoolManager.Inst.Set_TextCoinGet(transform, Vector3.zero);
        ui_textCoinGet.SetText(StaticManager.Get_BigintegerToString(base.value), Color.green);
        base.BreakFromDamage();
    }
}
