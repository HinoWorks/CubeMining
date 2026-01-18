using UnityEngine;

public class bulletCont_ThunderStrike : BulletBase
{
    private Vector3 offsetPosition = new Vector3(0, 5f, 0); // 雷発生位置のオフセット


    /// <summary>
    /// 雷発生, triggerではなく触接指定したブロックにダメージを与える
    /// </summary>
    public void Init(int _damage, MiningTargetBase _target)
    {
        damage = _damage;

        gameObject.SetActive(true);
        base.SetLifetime();


        var effUnit = EffectManager.Inst.Get_Effect(EffectType.ThunderStrike);
        effUnit.transform.position = transform.position + offsetPosition;
        effUnit.SetActive(true);

        _target.Damage(damage);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        // 何もしない
    }

}
