using UnityEngine;

public class BulletCont_Normal : BulletBase
{

    public override void Init(int _damage, float _lifetime, Vector3 _direction)
    {
        base.Init(_damage, _lifetime, _direction);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.TryGetComponent(out IDamagable target))
        {
            target.Damage(damage);
            if (bulletType == BulletType.Piercing) return;
            base.ReturnToPool();
        }
    }
}
