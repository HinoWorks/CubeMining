using UnityEngine;

public class BulletCont_ThunderStrike : BulletBase
{
    [SerializeField] EffectType effectType = EffectType.ThunderStrike;


    /// <summary>
    /// 雷発生, triggerではなく触接指定したブロックにダメージを与える
    /// </summary>
    public void Init(int _damage, MiningTargetBase _target)
    {
        damage = _damage;

        gameObject.SetActive(true);
        if (col == null)
        {
            ConnectComponents();
        }
        base.SetLifetime();

        var effUnit = EffectManager.Inst.Get_Effect(effectType);
        effUnit.transform.position = transform.position;
        effUnit.SetActive(true);

        _target.Damage(damage);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        // 何もしない
    }

}
