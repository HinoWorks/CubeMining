using UnityEngine;
using UniRx;
using System;

public class BulletCont_TurretUnit : MonoBehaviour
{
    [SerializeField] Transform rotateTarget;
    [SerializeField] TriggerSender[] triggerSender;
    protected int damage;
    protected float lifetime;
    protected float speed;

    void Awake()
    {
        foreach (var trigger in triggerSender)
        {
            trigger.OnEnter += OnTriggerEnter;
        }
    }
    public virtual void Init(int _damage, float _lifetime, float _speed)
    {
        damage = _damage;
        lifetime = _lifetime;
        speed = _speed;

        gameObject.SetActive(true);
        SetLifetime();
    }
    public virtual void ReturnToPool()
    {
        this.gameObject.SetActive(false);
    }

    public virtual void OnDestroy()
    {
        Destroy(this.gameObject);
    }

    protected virtual void SetLifetime()
    {
        Observable.Timer(TimeSpan.FromSeconds(lifetime))
            .Subscribe(_ =>
            {
                this.gameObject.SetActive(false);
            }).AddTo(this);
    }

    void Update()
    {
        transform.RotateAround(rotateTarget.position, Vector3.up, speed * 10 * Time.deltaTime);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamagable target))
        {
            target.Damage(damage);
        }
    }






}
