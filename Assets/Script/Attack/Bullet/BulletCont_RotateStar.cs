using UnityEngine;
using DG.Tweening;

public class BulletCont_RotateStar : BulletBase
{

    public override void Init(int _damage, float _lifetime, Vector3 _direction)
    {
        base.SetBulletType(BulletType.Piercing);
        base.Init(_damage);
        transform.DOLocalMove(_direction, _lifetime).SetEase(Ease.Linear).SetLink(this.gameObject).Play();
    }

}
