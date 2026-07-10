using UnityEngine;

public class MiningTarget_Bomb : MiningTarget_Object
{
    [SerializeField] float baseExplosionSize = 3f;
    [SerializeField] private GameObject pf_bomb;
    [SerializeField] private ForceFieldArea forceFieldArea;
    private int breakCount = 2; //  2回ダメージを受けると爆発
    private int index_SE_Damage => 24;
    private int index_SE_Break => 25;

    public override void Init(ObjectGenerateParam _objectGenerateParam, BlockData _blockData)
    {
        base.Init(_objectGenerateParam, _blockData);

        var hp = (int)(_blockData.hp * _objectGenerateParam.so.hpRate);
        base.Init_MiningTargetBase(hp, 0, _objectGenerateParam.so.objectIndex);
    }


    public override bool Damage(int damage, float _resourceUpRate = 1f)
    {
        var damageFixed = base.hp_max / breakCount;
        var isBreak = base.Damage(damageFixed, _resourceUpRate);
        Set_BlockMesh();
        return isBreak;
    }

    public override void BreakFromDamage(float _resourceUpRate = 1f)
    {
        CameraManager.Inst?.ShakeCamera_BlockBreak();

        // explosion damage to surrounding blocks
        var newBomb = Instantiate(pf_bomb, InGameManager.Inst.ParentPool) as GameObject;
        var bomb = newBomb.GetComponent<MiningTarget_BombAttackArea>();
        bomb.transform.position = transform.position;

        var damageRate = objectGenerateParam != null ? objectGenerateParam.damageRate_total : 1f;
        var valueRate = objectGenerateParam != null ? objectGenerateParam.valueRate_total : 1f;
        var damage = AttackManager.Inst.currentPickaxeDamage
                        * damageRate
                        * (1f + ArtifactManager.Inst.bomb_damageRate);
        var sizeRate = valueRate + ArtifactManager.Inst.bomb_sizeRate;
        Debug.Log($"bomb damage => base:{AttackManager.Inst.currentPickaxeDamage} DamageUpRate:{damageRate} => damage:{damage}");
        Debug.Log($"bomb size => base:{baseExplosionSize} SizeUpRate:{sizeRate} => size:{baseExplosionSize * sizeRate}");
        bomb.Explode(damage, baseExplosionSize * sizeRate);
        forceFieldArea.Activate(ForceFieldArea.ForceType.Blast, transform.position, sizeRate);

        GameEvent.InGame.PublishGameRecordDataMod_Ingame(GameRecordData_Type.Damage, hp_max);

        base.BreakFromDamage(_resourceUpRate);
    }

    protected override void PlaySE_Damage()
    {
        SoundManager.Inst.PlaySE(index_SE_Damage);
    }

    protected override void PlaySE_Break()
    {
        SoundManager.Inst.PlaySE(index_SE_Break);
    }
}

