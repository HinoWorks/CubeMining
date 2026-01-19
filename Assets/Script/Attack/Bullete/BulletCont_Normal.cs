using UnityEngine;

public class BulletCont_Normal : BulletBase
{
    private TrailRenderer trailRenderer;


    public override void Init(int _damage, float _lifetime, Vector3 _direction)
    {
        if (trailRenderer == null)
        {
            trailRenderer = GetComponent<TrailRenderer>();
        }
        trailRenderer.Clear();

        base.Init(_damage, _lifetime, _direction);
    }
    public override void ReturnToPool()
    {
        base.ReturnToPool();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamagable target))
        {
            target.Damage(damage);
            if (bulletType == BulletType.Piercing) return;
            base.ReturnToPool();
        }
    }
}
