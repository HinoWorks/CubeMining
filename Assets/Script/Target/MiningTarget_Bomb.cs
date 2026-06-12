using UnityEngine;

public class MiningTarget_Bomb : MiningTarget_Object
{
    [SerializeField] private GameObject pf_bomb;

    private int index_SE_Damage => 24;
    private int index_SE_Break => 25;

    public override void Init(ObjectGenerateParam _objectGenerateParam, BlockData _blockData, int _layerIndex)
    {
        base.Init(_objectGenerateParam, _blockData, _layerIndex);

        var hp = (int)(_blockData.hp * _objectGenerateParam.so.hpRate);
        base.Init_MiningTargetBase(hp, 0, _objectGenerateParam.so.objectIndex, _layerIndex);
    }

    private const int ObjectIndex_Bomb = 3;

    public void Init_SkillBom(int _hp)
    {
        var bombParam = GameParamManager.list_objectGenerateParam.Find(x => x.so.objectIndex == ObjectIndex_Bomb);
        base.Init(bombParam, null, 0);
        base.Init_MiningTargetBase(_hp, 0, bombParam?.so.objectIndex ?? 0, 0);
        //Set_ActiveGravity();
    }

    public override void BreakFromDamage(float _resourceUpRate = 1f)
    {
        CameraManager.Inst?.ShakeCamera_BlockBreak();

        // explosion damage to surrounding blocks
        var newBomb = Instantiate(pf_bomb, InGameManager.Inst.ParentPool) as GameObject;
        var bomb = newBomb.GetComponent<MiningTarget_BombAttackArea>();
        bomb.transform.position = transform.position;

        var damageRate = objectGenerateParam != null ? objectGenerateParam.damageRate_total : 1f;
        var valueRate = objectGenerateParam != null ? objectGenerateParam.valueRate_total : 0f;
        var damage = AttackManager.Inst.currentPickaxeDamage
                        * damageRate
                        * (1f + ArtifactManager.Inst.bomb_damageRate);
        var size = valueRate + ArtifactManager.Inst.bomb_sizeRate;
        Debug.Log($"bomb damage => base:{AttackManager.Inst.currentPickaxeDamage} DamageUpRate:{damageRate} => damage:{damage}");
        bomb.Explode(damage, size);

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

