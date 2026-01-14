using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System;


public class AttackCont_BulletShot : AttackContBase
{
    [SerializeField] GameObject bulletPrefab;
    private List<BulletBase> bullets = new List<BulletBase>();


    // loc
    private int damage => (int)base.attackParam.damage;
    private float attackInterval => base.attackParam.attackInterval;
    private float speed => base.attackParam.speed;
    private float aliveTime => base.attackParam.aliveTime;
    private int count => base.attackParam.count + 3;
    private float deltaAngle => 360f / count;
    private Vector3 pointerPosition;
    private Vector3 offsetPosition = new Vector3(0, 0.1f, 0);




    protected override void AwakeCall()
    {
        //GameEvent.Input.PointerAreaIn.Subscribe(isAreaIn => PointerAreaIn(isAreaIn)).AddTo(this);
        GameEvent.Input.PointerMove.Subscribe(pos => PointerMove(pos)).AddTo(this);
    }
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
            .AddTo(this); // Destroy で自動終了
    }


    private void CreateBullet()
    {
        var initialAngle = UnityEngine.Random.Range(0f, 360f);
        for (int i = 0; i < count; i++)
        {
            var freeBullet = bullets.Find(x => !x.gameObject.activeSelf);
            if (freeBullet == null)
            {
                var newBullet = Instantiate(bulletPrefab, InGameManager.Inst.ParentPool) as GameObject;
                freeBullet = newBullet.GetComponent<BulletBase>();
                bullets.Add(freeBullet);
            }
            freeBullet.transform.position = transform.position;

            var direction = new Vector3(Mathf.Cos((initialAngle + i * deltaAngle) * Mathf.Deg2Rad), 0,
                                        Mathf.Sin((initialAngle + i * deltaAngle) * Mathf.Deg2Rad)) * speed;
            freeBullet.Init(damage, aliveTime, direction);
        }
    }


    #region -- position fix --
    private void PointerMove(Vector3 pos)
    {
        pointerPosition = pos + offsetPosition;
        transform.position = pointerPosition;
    }
    #endregion


}
