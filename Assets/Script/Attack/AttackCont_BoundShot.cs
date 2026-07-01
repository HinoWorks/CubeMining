using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System;

public class AttackCont_BoundShot : AttackContBase
{
    [SerializeField] GameObject bulletPrefab;
    private List<BulletCont_BoundShot> bullets = new List<BulletCont_BoundShot>();
    private Vector3 offsetPosition = new Vector3(0, 0.35f, 0);


    [Space(5)]
    [Header("Level2 checkRate")]
    [SerializeField] private float rate_Level2Unit = 0.3f;
    private bool isLevel2Unit => UnityEngine.Random.Range(0f, 1f) < rate_Level2Unit;
    private float speedRate_Level2Unit = 2f; // level2 速度をxx倍にする


    protected override void AwakeCall() { }

    public override void Init(AttackParam _attackParam)
    {
        base.Init(_attackParam);
        CreateAttackRoop();
    }

    public override void OnDestroy()
    {
        foreach (var bullet in bullets)
        {
            bullet.OnDestroy();
        }
        bullets.Clear();
        base.OnDestroy();
    }

    private void CreateAttackRoop()
    {
        Observable.Interval(TimeSpan.FromSeconds(attackInterval))
            .Where(_ => base.isActive)
            .Subscribe(_ =>
            {
                CreateBullet();
            })
            .AddTo(this);
    }

    private void CreateBullet()
    {
        for (int i = 0; i < count; i++)
        {
            var freeBullet = bullets.Find(x => !x.gameObject.activeSelf);
            if (freeBullet == null)
            {
                var newBullet = Instantiate(bulletPrefab, InGameManager.Inst.ParentPool) as GameObject;
                freeBullet = newBullet.GetComponent<BulletCont_BoundShot>();
                bullets.Add(freeBullet);
            }

            var randomAngle = UnityEngine.Random.Range(0f, 360f);
            var direction = new Vector3(
                Mathf.Cos(randomAngle * Mathf.Deg2Rad),
                0f,
                Mathf.Sin(randomAngle * Mathf.Deg2Rad));

            freeBullet.transform.position = AttackManager.Inst.currentPickaxePosition + offsetPosition;
            var isLevel2 = isLevel2Unit;
            var setSpeedRate = isLevel2 ? speedRate_Level2Unit : 1;
            freeBullet.Init(damage, aliveTime, direction * speed * setSpeedRate);
            freeBullet.SetLevelUnit_Level2(isLevel2);
        }
    }


}
