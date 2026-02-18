using UnityEngine;
using UniRx;
using System;

public class AttackCont_BowUnit : MonoBehaviour
{
    private AttackCont_Bow bowCont;
    private int damage;
    private float lifetime;
    private float speed;
    private Vector3 direction;

    public void Set_BowCont(AttackCont_Bow _bowCont)
    {
        bowCont = _bowCont;
    }
    public void Init(int _damage, float _lifetime, float _speed, Vector3 _direction)
    {
        this.damage = _damage;
        this.lifetime = _lifetime;
        this.speed = _speed;
        this.direction = _direction;

        Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ => ShotArrow()).AddTo(this);
        Observable.Timer(TimeSpan.FromSeconds(lifetime)).Subscribe(_ => ReturnToPool()).AddTo(this);
        this.gameObject.SetActive(true);
    }

    private void ReturnToPool()
    {
        this.gameObject.SetActive(false);
    }

    private void ShotArrow()
    {
        var freeArrow = bowCont.Get_FreeArrow();
        freeArrow.transform.position = transform.position;
        freeArrow.Init(damage, 2.5f, speed * direction);
    }
}
