using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System;



public class AttackCont_Laser : AttackContBase
{
    [SerializeField] GameObject bulletPrefab;
    private List<BulletCont_Laser> bullets = new List<BulletCont_Laser>();


    protected override void AwakeCall()
    {
        //GameEvent.Input.PointerAreaIn.Subscribe(isAreaIn => PointerAreaIn(isAreaIn)).AddTo(this);
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
        // レーザーは１つのみ
        var freeBullet = bullets.Find(x => !x.gameObject.activeSelf);
        if (freeBullet == null)
        {
            var newBullet = Instantiate(bulletPrefab, InGameManager.Inst.ParentPool) as GameObject;
            freeBullet = newBullet.GetComponent<BulletCont_Laser>();
            bullets.Add(freeBullet);
        }
        var setPosition = Vector3.zero;
        var targetBlock = BlockGenerateManager.Inst.Get_RandomTargetBlock();
        if (targetBlock != null)
        {
            setPosition = targetBlock.transform.position;
        }
        freeBullet.transform.position = new Vector3(setPosition.x, 0f, setPosition.z);
        freeBullet.Init(damage, aliveTime, count);
    }



}
