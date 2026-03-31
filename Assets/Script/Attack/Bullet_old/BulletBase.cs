using UnityEngine;
using UniRx;
using System;



public enum BulletType
{
    Normal,
    Piercing,
    Explosion,
}




public class BulletBase : MonoBehaviour
{
    protected int damage;
    protected float lifetime;

    protected Collider col;
    protected Rigidbody rb;



    protected BulletType bulletType;


    protected void ConnectComponents()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }


    public virtual void Init(int _damage, float _lifetime, Vector3 _direction)
    {
        Init(_damage, _direction);
        SetLifetime();
    }

    public virtual void Init(int _damage, Vector3 _direction)
    {
        damage = _damage;
        gameObject.SetActive(true);
        if (col == null)
        {
            ConnectComponents();
        }
        col.enabled = true;
        rb.linearVelocity = _direction;
        rb.angularVelocity = Vector3.zero;
        if (_direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(_direction);
    }
    public virtual void Init(int _damage)
    {
        damage = _damage;
        gameObject.SetActive(true);
        if (col == null)
        {
            ConnectComponents();
        }
        col.enabled = true;
        rb.angularVelocity = Vector3.zero;
    }

    public virtual void ReturnToPool()
    {
        col.enabled = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        this.gameObject.SetActive(false);
    }

    public virtual void OnDestroy()
    {
        if (col != null)
        {
            col.enabled = false;
        }
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        Destroy(this.gameObject);
    }

    public virtual void SetBulletType(BulletType _bulletType)
    {
        bulletType = _bulletType;
    }

    protected virtual void SetLifetime()
    {
        Observable.Timer(TimeSpan.FromSeconds(lifetime))
            .Subscribe(_ =>
            {
                col.enabled = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

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
