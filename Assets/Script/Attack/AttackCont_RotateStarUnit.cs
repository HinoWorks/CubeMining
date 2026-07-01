using UnityEngine;
using UniRx;
using System;


public enum RotateStarUnitType
{
    Normal,
    Vertical_1,
    Vertical_2
}

public class AttackCont_RotateStarUnit : MonoBehaviour
{
    private int damage;
    private float lifetime;
    private float speed;

    private const float initialRadius = 1f;
    private const float expandRadius = 3f;
    private float expandRadiusRate_Level2 = 1f;
    private bool isLevel2 => expandRadiusRate_Level2 > 1f;
    [SerializeField] BulletCont_RotateStar[] bulletCont_RotateStars;


    private RotateStarUnitType rotateStarUnitType;
    private Vector3 baseRotate_normal = Vector3.zero;
    //private Vector3 baseRotate_vertical_1 = new Vector3(0f, 90, 90f);
    //private Vector3 baseRotate_vertical_2 = new Vector3(0f, 180, 90f);


    public void Init(int _damage, float _lifetime, float _speed, int _starCount, float _expandRadiusRate)
    {
        this.damage = _damage;
        this.lifetime = _lifetime;
        this.speed = _speed;
        this.expandRadiusRate_Level2 = _expandRadiusRate;

        rotateStarUnitType = RotateStarUnitType.Normal;
        transform.localRotation = Quaternion.Euler(baseRotate_normal);

        CreateBullet(_starCount);

        Observable.Timer(TimeSpan.FromSeconds(lifetime)).Subscribe(_ => ReturnToPool()).AddTo(this);
        this.gameObject.SetActive(true);
    }

    void Update()
    {
        if (rotateStarUnitType == RotateStarUnitType.Normal)
        {
            transform.Rotate(Vector3.up, speed * Time.deltaTime);
        }
        else
        {

            transform.Rotate(Vector3.up, speed * Time.deltaTime);
        }
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
            bulletCont.SetLevelUnit_Level2(isLevel2);
            bulletCont.Init(damage, lifetime, setDirection * expandRadius * expandRadiusRate_Level2);
        }
    }


    private void ReturnToPool()
    {
        foreach (var bulletCont in bulletCont_RotateStars)
        {
            if (!bulletCont.gameObject.activeSelf) continue;
            bulletCont.ReturnToPool();
        }

        Observable.Timer(TimeSpan.FromSeconds(0.1f)).Subscribe(_ =>
        {
            this.gameObject.SetActive(false);
        }).AddTo(this);
    }



    /*
        public void Init_Vertical(int _damage, float _lifetime, float _speed, int _starCount)
        {
            this.damage = _damage;
            this.lifetime = _lifetime;
            this.speed = _speed;

            rotateStarUnitType = UnityEngine.Random.Range(0, 2) == 0 ? RotateStarUnitType.Vertical_1 : RotateStarUnitType.Vertical_2;
            var setRotate = rotateStarUnitType == RotateStarUnitType.Vertical_1 ? baseRotate_vertical_1 : baseRotate_vertical_2;
            transform.localRotation = Quaternion.Euler(setRotate);

            CreateBullet(_starCount);

            Observable.Timer(TimeSpan.FromSeconds(lifetime)).Subscribe(_ => ReturnToPool()).AddTo(this);
            this.gameObject.SetActive(true);
        }
    */
}
