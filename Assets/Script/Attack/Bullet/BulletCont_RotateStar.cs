using UnityEngine;
using DG.Tweening;

public class BulletCont_RotateStar : BulletBase
{
    [SerializeField] GameObject obj_level1;
    [SerializeField] GameObject obj_level2;
    private float size = 1f;



    public override void Init(int _damage, float _lifetime, Vector3 _direction)
    {
        this.gameObject.transform.localScale = Vector3.zero;

        base.SetBulletType(BulletType.Piercing);
        base.Init(_damage);
        transform.DOLocalMove(_direction, _lifetime).SetEase(Ease.Linear).SetLink(this.gameObject).Play();

        transform.DOScale(Vector3.one * size, _lifetime).SetEase(Ease.OutBack).Play();
    }

    public void SetLevelUnit_Level2(bool _isLevel2)
    {
        obj_level1.SetActive(!_isLevel2);
        obj_level2.SetActive(_isLevel2);
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
