using UnityEngine;
using UniRx;
using System;

public class BulletCont_IceCircle : MonoBehaviour
{
    [SerializeField] GameObject eff_IceCircle_1;
    [SerializeField] GameObject eff_IceCircle_2;
    [SerializeField] private float radius = 1f;
    [SerializeField] private LayerMask targetLayerMask;
    protected int damage;
    protected float GetRadius => radius * radiusRate;
    private float radiusRate = 1f;
    private float damageEndDelay = 0.5f;


    public virtual void Init(int _damage, float _radiusRate, int _level)
    {
        this.damage = _damage;
        this.radiusRate = _radiusRate; ;
        gameObject.SetActive(true);
        transform.rotation = Quaternion.identity;
        eff_IceCircle_1.SetActive(_level == 1);
        eff_IceCircle_2.SetActive(_level == 2);
        StartDelayedCircleDamage();
    }
    public virtual void Init(int _damage)
    {
        this.damage = _damage;
        gameObject.SetActive(true);
        StartDelayedCircleDamage();
    }


    protected void StartDelayedCircleDamage()
    {
        if (!gameObject.activeInHierarchy) return;
        DamageTargetsInCircle();

        Observable.Timer(TimeSpan.FromSeconds(damageEndDelay))
            .Subscribe(_ =>
            {
                ReturnToPool();
            }).AddTo(this);
    }

    protected void DamageTargetsInCircle()
    {
        var colliders = Physics.OverlapSphere(transform.position, GetRadius, targetLayerMask);
        foreach (var hitCol in colliders)
        {
            if (hitCol == null) continue;
            if (hitCol.TryGetComponent(out IDamagable target) && target.isAlive)
            {
                target.Damage(damage);
            }
        }
    }


    public virtual void ReturnToPool()
    {
        this.gameObject.SetActive(false);
    }

    public virtual void OnDestroy()
    {
        Destroy(this.gameObject);
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.4f, 0.8f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, GetRadius);
    }
#endif
}
