using UnityEngine;
using System;
using Unity.Mathematics;
using UniRx.Triggers;
public class MiningTarget_Object : MiningTargetBase
{
    [SerializeField] GameObject[] obj_blockMeshes;
    protected Vector3 EffectOffset = new Vector3(0, 0.25f, 0);
    protected ObjectGenerateParam objectGenerateParam;
    protected Action breakCallback;

    // -- mesh --
    private float meshThreshold_1 = 0.7f;
    private float meshThreshold_2 = 0.35f;



    public void Set_BreakCallback(Action _callback)
    {
        breakCallback = _callback;
    }
    public override void NotActivate()
    {
        base.NotActivate();
        breakCallback = null;
    }


    public virtual void Init(ObjectGenerateParam _objectGenerateParam, BlockData _blockData, int _layerIndex)
    {
        objectGenerateParam = _objectGenerateParam;
        layerIndex = _layerIndex;
        transform.localRotation = Quaternion.identity;
    }

    protected void Init_MiningTargetBase(int _hp, int _value, int _index, int _layerIndex)
    {
        base.Init(_hp, _value, 1f);
        base.animScale_rate = this.transform.localScale.x;
        Set_BlockMesh();
    }

    protected virtual void Set_BlockMesh()
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
    /*
    public void Set_ActiveGravity()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
    }
    */


    public override bool Damage(int damage, float _resourceUpRate = 1f)
    {
        var isBreak = base.Damage(damage, _resourceUpRate);
        Set_BlockMesh();
        return isBreak;
    }

    public override void BreakFromDamage(float _resourceUpRate = 1f)
    {
        //rb.isKinematic = true;
        //rb.useGravity = false;

        base.BreakFromDamage();
        breakCallback?.Invoke();
    }
}
