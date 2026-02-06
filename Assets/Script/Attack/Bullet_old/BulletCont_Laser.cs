using UnityEngine;
using UniRx;
using System;

public class BulletCont_Laser : MonoBehaviour
{
    [SerializeField] TriggerSender[] triggerSender;
    [SerializeField] Rigidbody[] rbs;
    protected int damage;
    protected float lifetime;

    private float setSpped = 10f;

    void Awake()
    {
        foreach (var trigger in triggerSender)
        {
            trigger.OnEnter += OnTriggerEnter;
        }
    }

    public virtual void OnDestroy()
    {
        foreach (var trigger in triggerSender)
        {
            trigger.OnEnter -= OnTriggerEnter;
        }
        Destroy(this.gameObject);
    }
    public void Init(int _damage, float _lifetime, int _count, bool _isVertical)
    {
        damage = _damage;
        lifetime = _lifetime;

        gameObject.SetActive(true);
        SetLifetime();
        SetBullet(_count, _isVertical);
    }

    private void SetBullet(int _count, bool _isVertical)
    {
        var offsetAngle = _isVertical ? 90f : 0f;
        foreach (var rb in rbs)
        {
            rb.gameObject.SetActive(false);
            rb.transform.localPosition = Vector3.zero;
        }
        for (int i = 0; i < _count; i++)
        {
            rbs[i].gameObject.SetActive(true);
            var setAngle = i * 360f / _count + offsetAngle;
            var setDirection = new Vector3(Mathf.Cos(setAngle * Mathf.Deg2Rad), 0, Mathf.Sin(setAngle * Mathf.Deg2Rad));
            rbs[i].linearVelocity = setSpped * setDirection;
        }
    }
    public virtual void ReturnToPool()
    {
        this.gameObject.SetActive(false);
    }

    protected virtual void SetLifetime()
    {
        Observable.Timer(TimeSpan.FromSeconds(lifetime))
            .Subscribe(_ =>
            {
                this.gameObject.SetActive(false);
            }).AddTo(this);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamagable target))
        {
            target.Damage(damage);
        }
    }

}
