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

    private Collider col;
    private Rigidbody rb;


    protected BulletType bulletType;



    void Awake()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }


    public virtual void Init(int _damage, float _lifetime, Vector3 _direction)
    {
        damage = _damage;
        lifetime = _lifetime;

        gameObject.SetActive(true);
        col.enabled = true;
        rb.linearVelocity = _direction;
        rb.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.LookRotation(_direction);

        SetLifetime();
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
        col.enabled = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        Destroy(this.gameObject);
    }

    protected virtual void SetBulletType(BulletType _bulletType)
    {
        bulletType = _bulletType;
    }

    private void SetLifetime()
    {
        Observable.Timer(TimeSpan.FromSeconds(lifetime))
            .Subscribe(_ =>
            {
                col.enabled = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                this.gameObject.SetActive(false);
            });
    }



    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamagable target))
        {
            target.Damage(damage);
        }
    }




}
