using UnityEngine;

public class MiningTarget_Bomb_PickaxePower : MiningTarget_Object
{
    [SerializeField] private GameObject pf_bomb;

    private int breakCount = 3; //  3回ダメージを受けると爆発
    private int index_SE_Damage => 24;
    private int index_SE_Break => 25;

    private int damage;
    private float sizeRate;

    private const int ObjectIndex_Bomb = 3;

    public void Init_SkillBom(int _hp, int _damage, float _sizeRate)
    {
        damage = _damage;
        sizeRate = _sizeRate;
        var bombParam = GameParamManager.list_objectGenerateParam.Find(x => x.so.objectIndex == ObjectIndex_Bomb);
        base.Init(bombParam, null, 0);
        base.Init_MiningTargetBase(_hp, 0, bombParam?.so.objectIndex ?? 0, 0);
        Set_ActiveGravity();
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

        bomb.Explode(damage, sizeRate);
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
