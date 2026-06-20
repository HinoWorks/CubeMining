using UnityEngine;
using DG.Tweening;

public class BulletCont_RotateStar : BulletBase
{
    private float size = 1f;
    public override void Init(int _damage, float _lifetime, Vector3 _direction)
    {
        this.gameObject.transform.localScale = Vector3.zero;

        base.SetBulletType(BulletType.Piercing);
        base.Init(_damage);
        transform.DOLocalMove(_direction, _lifetime).SetEase(Ease.Linear).SetLink(this.gameObject).Play();

        transform.DOScale(Vector3.one * size, _lifetime).SetEase(Ease.OutBack).Play();
    }
    public void SetSize(float _size)
    {
        this.size = _size;
    }

    public override void ReturnToPool()
    {
        transform.DOKill();
        this.gameObject.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                base.ReturnToPool();
                transform.DOKill();
            });
    }

}
