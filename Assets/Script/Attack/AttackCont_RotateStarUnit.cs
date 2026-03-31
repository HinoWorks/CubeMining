using UnityEngine;
using UniRx;
using System;

public class AttackCont_RotateStarUnit : MonoBehaviour
{
    private int damage;
    private float lifetime;
    private float speed;

    private float initialRadius = 1f;
    private float expandRadius = 3f;
    [SerializeField] BulletCont_RotateStar[] bulletCont_RotateStars;



    public void Init(int _damage, float _lifetime, float _speed, int _starCount)
    {
        this.damage = _damage;
        this.lifetime = _lifetime;
        this.speed = _speed;
        transform.localRotation = Quaternion.identity;

        CreateBullet(_starCount);

        Observable.Timer(TimeSpan.FromSeconds(lifetime)).Subscribe(_ => ReturnToPool()).AddTo(this);
        this.gameObject.SetActive(true);
    }

    void Update()
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
    }

    public void OnDestroy()
    {
        Destroy(this.gameObject);
    }

    private void CreateBullet(int _count)
    {
        foreach (var bulletCont in bulletCont_RotateStars)
        {
            bulletCont.gameObject.SetActive(false);
        }
        var deltaAngle = 360f / _count * Mathf.Deg2Rad;
        for (int i = 0; i < _count; i++)
        {
            var bulletCont = bulletCont_RotateStars[i];

            var setDirection = new Vector3(Mathf.Cos(i * deltaAngle), 0f, Mathf.Sin(i * deltaAngle));
            bulletCont.transform.localPosition = setDirection * initialRadius;
            bulletCont.Init(damage, lifetime, setDirection * expandRadius);
        }
    }


    private void ReturnToPool()
    {
        foreach (var bulletCont in bulletCont_RotateStars)
        {
            if (!bulletCont.gameObject.activeSelf) continue;
            bulletCont.ReturnToPool();
        }
        this.gameObject.SetActive(false);
    }

}
